using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.Register.Commands;

public record CreateApplicationUserCommand(string FullName,string Email,string Password)
    : ICommand<RequestResponse<CreateApplicationUser>>
{
    
}
