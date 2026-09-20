using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.ForgotPassword.Commands
{
    public record ForgetPasswordCommand(string Email) : ITransactionalCommand<RequestResponse>
    {
    }
}
