using exam_system.Features.Identity.VerifyEmailOtp.Commands;
using exam_system.Features.Identity.VerifyEmailOtp.Dtos.Response;
using exam_system.Features.Identity.VerifyEmailOtp.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Identity.VerifyEmailOtp.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class VerifyEmailOtpController : ControllerBase
{
    private readonly IMediator _mediator;

    public VerifyEmailOtpController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailOtpViewModel viewModel,
        CancellationToken cancellationToken)
    {
        var command = new VerifyEmailOtpCommand(
            viewModel.Email,
            viewModel.Otp);

        var result = await _mediator.Send(command, cancellationToken);
        var response = EndpointResponse<VerifyEmailOtpResponse>.FromResult(result);

        return StatusCode(response.StatusCode, response);
    }
}
