using exam_system.Features.Diplomas.AdminUpdateDiploma.Commands;
using exam_system.Features.Diplomas.AdminUpdateDiploma.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateDiplomaController : ControllerBase
    {
        IMediator _mediator;
        public UpdateDiplomaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update( Guid id,[FromBody] UpdateDiplomaDto dto)
        {
            var command = new UpdateDiplomaCommand(
                id,
                dto.Title,
                dto.Description);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

    }
}
