using exam_system.Features.Identity.Logout.Commands;
using exam_system.Features.Identity.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Identity.Logout.Controllers;

[Route("api/auth")]
[ApiController]
[AllowAnonymous]
public sealed class LogoutController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogoutController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";

        var command = new LogoutCommand(RefreshTokenCookie.Read(Request));
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            // Keep the cookie on a server failure so revocation can be retried.
            RefreshTokenCookie.Delete(Response);
        }

        var response = EndpointResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}
