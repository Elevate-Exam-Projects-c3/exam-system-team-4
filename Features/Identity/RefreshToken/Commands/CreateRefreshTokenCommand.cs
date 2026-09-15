using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.RefreshTokens.Commands;

public sealed record CreateRefreshTokenCommand(
    string UserId,
    string RefreshToken,
    DateTimeOffset ExpiresAt)
    : ICommand<RequestResponse>;
