using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.RefreshTokens.Commands;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Services;
using exam_system.Persistence.Context;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Identity.RefreshTokens.Handlers;

public sealed class MarkRefreshTokenAsUsedCommandHandler
    : IRequestHandler<MarkRefreshTokenAsUsedCommand, RequestResponse<bool>>
{
    private readonly IGenericRepository<RefreshToken> _refreshTokens;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;

    public MarkRefreshTokenAsUsedCommandHandler(
        IGenericRepository<RefreshToken> refreshTokens,
        ITokenService tokenService,
        AppDbContext context)
    {
        _refreshTokens = refreshTokens;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<RequestResponse<bool>> Handle(
        MarkRefreshTokenAsUsedCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // This immediate update must commit together with the replacement
        // inserted later in the RefreshSessionCommand transaction.
        if (_context.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException(
                "Consuming a refresh token requires an active transaction. Send RefreshSessionCommand instead.");
        }

        var replacementTokenHash = _tokenService.HashRefreshToken(
            request.ReplacementRefreshToken);

        // Recheck validity in the update itself: the orchestrator's earlier
        // lookup cannot prevent another request from consuming this token.
        var affectedRows = await _refreshTokens
            .Get(token =>
                token.Id == request.RefreshTokenId &&
                !token.IsUsed &&
                !token.IsRevoked &&
                token.ExpiresAt > DateTime.UtcNow)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(token => token.IsUsed, true)
                .SetProperty(token => token.ReplacedByTokenHash, replacementTokenHash)
                .SetProperty(token => token.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        return RequestResponse<bool>.Ok(affectedRows == 1);
    }
}
