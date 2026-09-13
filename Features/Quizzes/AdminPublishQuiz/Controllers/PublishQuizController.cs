using exam_system.Features.Quizzes.AdminPublishQuiz.Orchestrators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Quizzes.AdminPublishQuiz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Admin")]
    public class PublishQuizController(IMediator mediator) : ControllerBase
    {
        [HttpPost("{quizId}/publish")]
        public async Task<IActionResult> Publish(
    Guid quizId,
    CancellationToken cancellationToken)
        {
           var result = await mediator.Send(
                new PublishQuizOrchestrator(quizId),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("{quizId}/unpublish")]
        public async Task<IActionResult> Unpublish(
    Guid quizId,
    CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new UnpublishQuizOrchestrator(quizId),
                cancellationToken);

            return Ok(result);
        }
    }


}
