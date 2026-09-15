using exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries;
using exam_system.Features.Quizzes.Readiness;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Admin")]
    public class QuizReadinessController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizReadinessController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{quizId:guid}/readiness")]
        public async Task<ActionResult<QuizReadinessDto>> CheckReadiness(
        Guid quizId,
        CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(
                new QuizReadinessQuery(quizId),
                cancellationToken);


            return Ok(result);
        }
    }
}
