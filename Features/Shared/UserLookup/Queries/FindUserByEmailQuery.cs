using exam_system.Domain.Entities.Identity;
using MediatR;

namespace exam_system.Features.Shared.UserLookup.Queries;

public sealed record FindUserByEmailQuery(string Email)
    : IRequest<RequestResponse<ApplicationUser?>>;
