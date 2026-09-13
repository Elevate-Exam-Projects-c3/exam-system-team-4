using exam_system.Features.Diplomas.EnrollDiploma.Orchestrators;
using exam_system.Features.Diplomas.EnrollDiploma.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.EnrollDiploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentEnrollDiplomaController : ControllerBase
    {
        private readonly EnrollDiplomaOrchestrator _orchestrator;

        public StudentEnrollDiplomaController(EnrollDiplomaOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost]
        public async Task<IActionResult> EnrollDiploma(  [FromBody] StudentEnrollmentDiplomaViewModel viewModel,  CancellationToken cancellationToken)
        {
            var result = await _orchestrator.ExecuteAsync( viewModel.StudentId, viewModel.DiplomaId,   cancellationToken);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }
    }
}