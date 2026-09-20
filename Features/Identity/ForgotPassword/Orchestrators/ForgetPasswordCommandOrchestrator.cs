using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Identity.ForgotPassword.Notification;
using exam_system.Features.Shared;
using exam_system.Features.Shared.UserLookup.Queries;
using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators
{
    public class ForgetPasswordCommandOrchestrator : IRequestHandler<ForgetPasswordCommand, RequestResponse>
    {
        private readonly IMediator _mediator;

        public ForgetPasswordCommandOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<RequestResponse> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var findUserResult = await _mediator.Send(new FindUserByEmailQuery(request.Email),cancellationToken);

            if (!findUserResult.Success)
            {
                return RequestResponse.Fail(
                    findUserResult.Message,
                    findUserResult.StatusCode,
                    findUserResult.Errors);
            }

            var user = findUserResult.Data;
            if (user is null || string.IsNullOrWhiteSpace(user.Email))
            {
                return RequestResponse.Ok("If this email is registered, a code has been sent");
            }

            var createOtpResult = await _mediator.Send(new CreateForgetPasswordOtpCommand(user.Id),cancellationToken);

            if (!createOtpResult.Success)
            {
                return RequestResponse.Fail(
                    createOtpResult.Message,
                    createOtpResult.StatusCode,
                    createOtpResult.Errors);
            }

            var otpData = createOtpResult.Data;

            if (otpData is not null)
            {
                await _mediator.Publish(new SendPasswordResetOtpNotification(user.Email,otpData.Otp),cancellationToken);
            }

            return RequestResponse.Ok( "If this email is registered, a code has been sent");
        }
    }
}
