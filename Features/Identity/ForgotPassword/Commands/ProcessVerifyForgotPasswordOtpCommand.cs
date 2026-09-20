using exam_system.Features.Identity.ForgotPassword.Response;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;
using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Commands
{
    public sealed record ProcessVerifyForgotPasswordOtpCommand(string UserId, string Otp)
      : ITransactionalCommand<RequestResponse<ProcessVerifyForgotPasswordOtpResult>>;
}
