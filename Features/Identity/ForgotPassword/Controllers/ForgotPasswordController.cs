using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Identity.ForgotPassword.Response;
using exam_system.Features.Identity.ForgotPassword.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace exam_system.Features.Identity.ForgotPassword.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[EnableRateLimiting("auth")]
public sealed class ForgotPasswordController : ControllerBase
{
    private readonly IMediator _mediator;

    public ForgotPasswordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("forgot-password/reset")]
    public async Task<IActionResult> ResetForgotPassword(
        [FromBody] ResetForgotPasswordViewModel viewModel,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ResetForgotPasswordCommand(
            viewModel.Email, viewModel.ResetToken, viewModel.NewPassword, viewModel.ConfirmPassword),
            cancellationToken);
        var response = EndpointResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("forgot-password/verify")]
    public async Task<IActionResult> VerifyForgotPasswordOtp(
        [FromBody] VerifyForgotPasswordOtpViewModel viewModel,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new VerifyForgotPasswordOtpCommand(viewModel.Email, viewModel.Otp),
            cancellationToken);
        var response = EndpointResponse<VerifyForgotPasswordOtpResponse>.FromResult(result);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordViewModel viewModel,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ForgetPasswordCommand(viewModel.Email), cancellationToken);
        var response = EndpointResponse.FromResult(result);

        return StatusCode(response.StatusCode, response);
    }
}
