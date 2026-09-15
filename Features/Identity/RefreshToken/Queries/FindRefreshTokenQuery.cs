using exam_system.Features.Identity.RefreshTokens.Dtos.Response;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Identity.RefreshTokens.Queries;

public sealed record FindRefreshTokenQuery(string RefreshToken)
    : IRequest<RequestResponse<RefreshTokenLookupResult?>>;
