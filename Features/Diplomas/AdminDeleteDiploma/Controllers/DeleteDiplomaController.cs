using exam_system.Features.Diplomas.AdminDeleteDiploma.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteDiplomaController : ControllerBase
    {
        IMediator _mediator;
        public DeleteDiplomaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> DeleteDiploma(Guid id)
        {
            var command = new DeleteDiplomaCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
            {
                return NotFound();
            }
            return Ok("Diploma deleted successfully.");
        }
    }
}
