using exam_system.Features.Identity.ForgotPassword.Response;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Commands
{
    public sealed record VerifyForgotPasswordOtpCommand( string Email, string Otp)
     : IRequest<RequestResponse<VerifyForgotPasswordOtpResponse>>;
}
