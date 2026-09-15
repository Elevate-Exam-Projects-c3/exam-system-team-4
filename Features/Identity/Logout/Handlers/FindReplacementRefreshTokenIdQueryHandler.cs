using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Logout.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Identity.Logout.Handlers;

public sealed class FindReplacementRefreshTokenIdQueryHandler
    : IRequestHandler<FindReplacementRefreshTokenIdQuery, RequestResponse<Guid?>>
{
    private readonly IGenericRepository<RefreshToken> _refreshTokens;

    public FindReplacementRefreshTokenIdQueryHandler(IGenericRepository<RefreshToken> refreshTokens)
    {
        _refreshTokens = refreshTokens;
    }

    public async Task<RequestResponse<Guid?>> Handle(
        FindReplacementRefreshTokenIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var previousTokens = _refreshTokens.Get(token => token.Id == request.RefreshTokenId);
        var replacementId = await _refreshTokens
            .GetAll()
            .AsNoTracking()
            .Where(replacement => previousTokens.Any(previous =>
                previous.ReplacedByTokenHash == replacement.TokenHash &&
                previous.UserId == replacement.UserId))
            .Select(replacement => (Guid?)replacement.Id)
            .SingleOrDefaultAsync(cancellationToken);

        return RequestResponse<Guid?>.Ok(replacementId);
    }
}
