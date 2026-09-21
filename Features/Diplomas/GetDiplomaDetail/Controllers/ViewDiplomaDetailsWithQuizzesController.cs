using exam_system.Features.Diplomas.GetDiplomaDetail.Queries;
using exam_system.Features.Diplomas.GetDiplomaDetail.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewDiplomaDetailsWithQuizzesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ViewDiplomaDetailsWithQuizzesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("{diplomaId:guid}")]
        public async Task<IActionResult> ViewDiplomaDetailsWithQuizzes(
            [FromRoute] Guid diplomaId)
        {
            var studentIdClaim = User.FindFirst("studentId")?.Value;

            if (studentIdClaim is null)
                return Unauthorized("StudentId claim is missing.");

            var studentId = Guid.Parse(studentIdClaim);

            var result = await _mediator.Send(
                new GetDiplomaQuery(diplomaId, studentId));

            if (result?.Data is null)
                return NotFound();

            var data = result.Data;

            var viewModel = new ViewDiplomaDetailsWithQuizzesViewModel
            {
                DiplomaId = data.DiplomaId,
                Title = data.Title,
                Description = data.Description,

                Quizzes = data.Quizzes.Select(quizViewMod => new QuizViewModel
                {
                    QuizId = quizViewMod.QuizId,
                    Title = quizViewMod.Title,
                    Duration = quizViewMod.Duration,
                    PassScore = quizViewMod.PassScore,
                    MaxAttempts = quizViewMod.MaxAttempts,
                    AttemptsUsed = quizViewMod.AttemptsUsed,
                    HasInProgressAttempt = quizViewMod.HasInProgressAttempt,
                    IsResumable= quizViewMod.IsResumable,

                    Attempts = quizViewMod.Attempts
                        .Select(attempt => new AttemptViewModel
                        {
                            AttemptId = attempt.AttemptId,
                            QuizId = attempt.QuizId,
                            StartTime = attempt.StartTime,
                            Deadline = attempt.Deadline,
                            DurationMinutes = attempt.DurationMinutes,
                        })
                        .ToList()

                }).ToList()
            };

            return Ok(viewModel);
        }
    }
}