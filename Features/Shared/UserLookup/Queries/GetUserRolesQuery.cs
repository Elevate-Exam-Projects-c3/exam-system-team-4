using MediatR;

namespace exam_system.Features.Shared.UserLookup.Queries;

public sealed record GetUserRolesQuery(string UserId)
    : IRequest<RequestResponse<IReadOnlyList<string>?>>;
