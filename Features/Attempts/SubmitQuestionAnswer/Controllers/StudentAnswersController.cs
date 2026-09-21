using exam_system.Features.Attempts.SubmitQuestionAnswer.DTOs;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAnswersController(IMediator mediator) : ControllerBase
    {

        [HttpPost("{attemptId:guid}/answers")]
        public async Task<IActionResult> SubmitAnswer(
        Guid attemptId,
        [FromBody] SubmitAnswerRequestViewModel request,
        CancellationToken cancellationToken)
        {
            var studentIdClaim = User.FindFirst("studentId")?.Value;

            if (!Guid.TryParse(studentIdClaim, out var studentId))
            {
                return Unauthorized();
            }

            var result = await mediator.Send(
                new SubmitAnswerOrchestrator(
                    attemptId,
                    studentId,
                    request.QuestionId,
                    request.SelectedOptionId),
                cancellationToken);

            return  Ok(result);
        }
    }
}
