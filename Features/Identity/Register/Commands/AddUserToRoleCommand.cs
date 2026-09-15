using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.Register.Commands;

public record AddUserToRoleCommand(string UserId): ICommand<RequestResponse>;