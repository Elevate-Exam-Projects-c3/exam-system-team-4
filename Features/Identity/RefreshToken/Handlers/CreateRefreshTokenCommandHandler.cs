using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.RefreshTokens.Commands;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Services;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Identity.RefreshTokens.Handlers;

public sealed class CreateRefreshTokenCommandHandler
    : IRequestHandler<CreateRefreshTokenCommand, RequestResponse>
{
    private readonly IGenericRepository<RefreshToken> _refreshTokens;
    private readonly ITokenService _tokenService;

    public CreateRefreshTokenCommandHandler(
        IGenericRepository<RefreshToken> refreshTokens,
        ITokenService tokenService)
    {
        _refreshTokens = refreshTokens;
        _tokenService = tokenService;
    }

    public Task<RequestResponse> Handle(
        CreateRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var token = new RefreshToken
        {
            UserId = request.UserId,
            TokenHash = _tokenService.HashRefreshToken(request.RefreshToken),
            ExpiresAt = request.ExpiresAt.UtcDateTime,
            IsUsed = false,
            IsRevoked = false
        };

        // The outer RefreshSessionCommand transaction saves this token
        // together with the update that consumes the previous token.
        _refreshTokens.Add(token);

        return Task.FromResult(RequestResponse.Ok());
    }
}
