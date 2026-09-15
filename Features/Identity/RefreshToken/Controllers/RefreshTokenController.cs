using exam_system.Features.Identity.RefreshTokens.Commands;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Identity.RefreshTokens.Controllers;

[Route("api/auth")]
[ApiController]
[AllowAnonymous]
public sealed class RefreshTokenController : ControllerBase
{
    private readonly IMediator _mediator;

    public RefreshTokenController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";

        var command = new RefreshSessionCommand(RefreshTokenCookie.Read(Request));
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            // Send returns after the transaction commits the token rotation.
            var tokens = result.Data!;
            RefreshTokenCookie.Append(Response, tokens.RefreshToken, tokens.RefreshTokenExpiresAt);
        }

        // TokenResult excludes the refresh token and its expiry from JSON.
        var response = EndpointResponse<TokenResult>.FromResult(result);

        return StatusCode(response.StatusCode, response);
    }
}
