using exam_system.Features.Diplomas.EnrollDiploma.Orchestrators;
using exam_system.Features.Diplomas.EnrollDiploma.ViewModels;
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
        public async Task<IActionResult> EnrollDiploma(  [FromBody] StudentEnrollmentDiplomaViewModel viewModel,  CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new EnrollDiplomaOrchestrator (viewModel.StudentId, viewModel.DiplomaId));

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }
    }
}