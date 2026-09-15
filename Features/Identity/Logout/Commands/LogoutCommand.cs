using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.Logout.Commands;

public sealed record LogoutCommand(string? RefreshToken)
    : ITransactionalCommand<RequestResponse>;
