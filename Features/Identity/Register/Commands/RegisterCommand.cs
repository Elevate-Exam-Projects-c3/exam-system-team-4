using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.Register.Commands
{
    public record RegisterCommand(string FullName, string Email, string Password)
    : ITransactionalCommand<RequestResponse<RegisterResponse>>
    {
      
    }

}
