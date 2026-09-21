using Azure;
using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Attempts.SubmitAttempt.Orchestrators;
using exam_system.Features.Attempts.SubmitAttempt.ViewModels;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Attempts.SubmitAttempt.Controllers
{
    [ApiController]
    [Route("api/attempts")]
    [Authorize]
    public class SubmitQuizAttemptController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubmitQuizAttemptController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{attemptId:guid}/submit")]
        public async Task<ActionResult<RequestResponse<SubmitAttemptResponseDto>>> Submit(
            Guid attemptId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new SubmitQuizAttemptOrchestrator(attemptId),
                cancellationToken);

            if (result is not null && result.Success && result.Data is not null)
            {
                var viewModel = new SubmitAttemptViewModel
                {
                    AttemptId = result.Data.AttemptId,
                    QuizId = result.Data.QuizId,
                    AttemptStatus = result.Data.AttemptStatus,
                    TotalQuestions = result.Data.TotalQuestions,
                    CorrectAnswers = result.Data.CorrectAnswers,
                    Score = result.Data.Score,
                    Passed = result.Data.Passed,
                    SubmittedAt = result.Data.SubmittedAt
                };
                return StatusCode(result.StatusCode,viewModel);
            }


            return StatusCode(result?.StatusCode ?? 500, result);

        }
    }
}