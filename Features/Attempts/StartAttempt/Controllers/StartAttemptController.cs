using exam_system.Features.Attempts.StartAttempt.Orchestrators;
using exam_system.Features.Attempts.StartAttempt.ViewModels;
using exam_system.Features.Quizzes.AdminCreateQuiz.ViewModels;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Attempts.StartAttempt.Controllers
{
    [ApiController]
    public class StartAttemptController:ControllerBase
    {
        private readonly IMediator _mediator;

        public StartAttemptController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("api/quizzes/{id}/attempts")]
        public async Task<IActionResult> StartAttempt(Guid id,Guid studentId  , CancellationToken cancellationToken)
        {
            
          var response= await _mediator.Send(new StartAttemptOrchestrator(id, studentId),cancellationToken);
            if(response is not null&& response.Success&&response.Data is not null)
            {
                var viewModel = new StartAttemptResponseViewModel
                {
                    AttemptId = response.Data.AttemptId,
                    QuizId = response.Data.QuizId,
                    StartTime = response.Data.StartTime,
                    Deadline = response.Data.Deadline,
                    DurationMinutes = response.Data.DurationMinutes,
                    Questions = response.Data.Questions.Select(question => new AttemptQuestionViewModel
                    {
                        Id = question.Id,
                        Text = question.Text,
                        Options = question.Options.Select(opt => new AttemptOptionViewModel
                        {
                            Id = question.Id,
                            Text = question.Text
                        }).ToList()
                    }).ToList()
                };
                return StatusCode(response.StatusCode, viewModel);
            }
           

            return StatusCode(response.StatusCode, response);

        }
    }
}
