using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;
using exam_system.Features.Shared.Services;

namespace exam_system.Features.Identity.RefreshTokens.Commands;

public sealed record RefreshSessionCommand(string? RefreshToken)
    : ITransactionalCommand<RequestResponse<TokenResult>>;
