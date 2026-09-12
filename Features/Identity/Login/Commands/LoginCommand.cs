using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;
using exam_system.Features.Shared.Services;

namespace exam_system.Features.Identity.Login.Commands;

public sealed record LoginCommand(string Email, string Password)
    : ICommand<RequestResponse<TokenResult>>;
