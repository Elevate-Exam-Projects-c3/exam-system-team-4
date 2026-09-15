using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Logout.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Identity.Logout.Handlers;

public sealed class RevokeRefreshTokenCommandHandler
    : IRequestHandler<RevokeRefreshTokenCommand, RequestResponse>
{
    private readonly IGenericRepository<RefreshToken> _refreshTokens;

    public RevokeRefreshTokenCommandHandler(IGenericRepository<RefreshToken> refreshTokens)
    {
        _refreshTokens = refreshTokens;
    }

    public async Task<RequestResponse> Handle(
        RevokeRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Update only revocation fields so a concurrent rotation's fields are preserved.
        await _refreshTokens
            .Get(token => token.Id == request.RefreshTokenId && !token.IsRevoked)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(token => token.IsRevoked, true)
                .SetProperty(token => token.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        // Already revoked or removed tokens require no further action.
        return RequestResponse.Ok();
    }
}
