using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Features.Shared.UserLookup.Queries;
using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators;

public sealed class ResetForgotPasswordCommandOrchestrator
    : IRequestHandler<ResetForgotPasswordCommand, RequestResponse>
{
    private readonly IMediator _mediator;

    public ResetForgotPasswordCommandOrchestrator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<RequestResponse> Handle(
        ResetForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var findUserResult = await _mediator.Send(
            new FindUserByEmailQuery(request.Email), cancellationToken);
        if (!findUserResult.Success)
        {
            return RequestResponse.Fail(
                findUserResult.Message, findUserResult.StatusCode, findUserResult.Errors);
        }
        if (findUserResult.Data is null)
        {
            return RequestResponse.Fail("Invalid or expired reset token.");
        }

        return await _mediator.Send(new ProcessResetForgotPasswordCommand(
            findUserResult.Data.Id, request.ResetToken, request.NewPassword), cancellationToken);
    }
}
