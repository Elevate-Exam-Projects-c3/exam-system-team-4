using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.RefreshTokens.Dtos.Response;
using exam_system.Features.Identity.RefreshTokens.Queries;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Services;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Identity.RefreshTokens.Handlers;

public sealed class FindRefreshTokenQueryHandler
    : IRequestHandler<FindRefreshTokenQuery, RequestResponse<RefreshTokenLookupResult?>>
{
    private readonly IGenericRepository<RefreshToken> _refreshTokens;
    private readonly ITokenService _tokenService;

    public FindRefreshTokenQueryHandler(
        IGenericRepository<RefreshToken> refreshTokens,
        ITokenService tokenService)
    {
        _refreshTokens = refreshTokens;
        _tokenService = tokenService;
    }

    public async Task<RequestResponse<RefreshTokenLookupResult?>> Handle(
        FindRefreshTokenQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var token = await _refreshTokens
            .Get(token => token.TokenHash == tokenHash)
            .AsNoTracking()
            .Select(token => new RefreshTokenLookupResult(
                token.Id,
                token.UserId,
                token.ExpiresAt,
                token.IsUsed,
                token.IsRevoked))
            .SingleOrDefaultAsync(cancellationToken);

        return RequestResponse<RefreshTokenLookupResult?>.Ok(token);
    }
}
