using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Identity.Logout.Queries;

public sealed record FindReplacementRefreshTokenIdQuery(Guid RefreshTokenId)
    : IRequest<RequestResponse<Guid?>>;
