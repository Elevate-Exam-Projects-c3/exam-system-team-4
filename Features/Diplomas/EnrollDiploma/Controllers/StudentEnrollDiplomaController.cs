using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.EnrollDiploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentEnrollDiplomaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentEnrollDiplomaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> EnrollDiploma([FromBody] StudentEnrollDiplomaCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
