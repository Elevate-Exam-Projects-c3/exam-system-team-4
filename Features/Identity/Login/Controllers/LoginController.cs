using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Identity.Login.View_Models;
using exam_system.Features.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Identity.Login.Controllers;

[Route("api/auth")]
[ApiController]
[AllowAnonymous]
public sealed class LoginController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel viewModel,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(viewModel.Email, viewModel.Password);
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Success)
        {
            var tokens = result.Data!;
            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth",
                Expires = tokens.RefreshTokenExpiresAt
            });
            Response.Headers.CacheControl = "no-store";
        }

        // TokenResult's refresh fields are excluded from JSON with JsonIgnore.
        var response = EndpointResponse<TokenResult>.FromResult(result);

        return StatusCode(response.StatusCode, response);
    }
}
