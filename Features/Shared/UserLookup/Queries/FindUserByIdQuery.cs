using exam_system.Features.Shared.UserLookup.Dtos.Response;
using MediatR;

namespace exam_system.Features.Shared.UserLookup.Queries;

public sealed record FindUserByIdQuery(string UserId)
    : IRequest<RequestResponse<UserLookupResult?>>;
