using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Dtos.response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Identity.Register.Controllers;

[ApiController]
[Route("api/auth")]
public class RegisterController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegisterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command,cancellationToken);

        var response = EndpointResponse<RegisterResponse>.FromResult(result);

        return StatusCode(response.StatusCode,response);
    }
}