using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Identity.ForgotPassword.Response;
using exam_system.Features.Shared;
using exam_system.Features.Shared.UserLookup.Queries;
using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators;

public sealed class VerifyForgotPasswordOtpCommandOrchestrator
    : IRequestHandler<VerifyForgotPasswordOtpCommand, RequestResponse<VerifyForgotPasswordOtpResponse>>
{
    private readonly IMediator _mediator;

    public VerifyForgotPasswordOtpCommandOrchestrator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<RequestResponse<VerifyForgotPasswordOtpResponse>> Handle(
        VerifyForgotPasswordOtpCommand request,
        CancellationToken cancellationToken)
    {
        var findUserResult = await _mediator.Send(
            new FindUserByEmailQuery(request.Email), cancellationToken);

        if (!findUserResult.Success)
        {
            return RequestResponse<VerifyForgotPasswordOtpResponse>.Fail(
                findUserResult.Message, findUserResult.StatusCode, findUserResult.Errors);
        }

        if (findUserResult.Data is null)
        {
            return RequestResponse<VerifyForgotPasswordOtpResponse>.Fail("Invalid code.");
        }

        // The inner transaction commits the attempt before it becomes an API error.
        var result = await _mediator.Send(
            new ProcessVerifyForgotPasswordOtpCommand(findUserResult.Data.Id, request.Otp),
            cancellationToken);

        if (!result.Success)
        {
            return RequestResponse<VerifyForgotPasswordOtpResponse>.Fail(
                result.Message, result.StatusCode, result.Errors);
        }

        return result.Data?.Status switch
        {
            PasswordResetOtpVerificationStatus.Verified when result.Data.ResetToken is not null =>
                RequestResponse<VerifyForgotPasswordOtpResponse>.Ok(
                    new VerifyForgotPasswordOtpResponse(result.Data.ResetToken), "Code verified."),
            PasswordResetOtpVerificationStatus.CodeExpired =>
                RequestResponse<VerifyForgotPasswordOtpResponse>.Fail("Code expired."),
            PasswordResetOtpVerificationStatus.AttemptsExceeded =>
                RequestResponse<VerifyForgotPasswordOtpResponse>.Fail(
                    "Attempt limit reached. Request a new code."),
            _ => RequestResponse<VerifyForgotPasswordOtpResponse>.Fail("Invalid code.")
        };
    }
}
