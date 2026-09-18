using exam_system.Features.Attempts.StartAttempt.Orchestrators;
using exam_system.Features.Attempts.StartAttempt.ViewModels;
using exam_system.Features.Quizzes.AdminCreateQuiz.ViewModels;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpPost("api/quizzes/{id}/attempts")]
        public async Task<IActionResult> StartAttempt(Guid id, CancellationToken cancellationToken)
        {
            
          var response= await _mediator.Send(new StartAttemptOrchestrator(id),cancellationToken);
            if(response is not null&& response.Success&&response.Data is not null)
            {
                var viewModel = new StartAttemptResponseViewModel
                {
                    AttemptId = response.Data.AttemptId,
                    QuizId = response.Data.QuizId,
                    StartTime = response.Data.StartTime,
                    Deadline = response.Data.Deadline,
                    LastAnsweredQuestionId=response.Data.LastAnsweredQuestionId,
                    DurationMinutes = response.Data.DurationMinutes,
                    
                    Questions = response.Data.Questions.Select(question => new AttemptQuestionViewModel
                    {
                        Id = question.Id,
                        Text = question.Text,
                        SelectedOptionId = question.SelectedOptionId,
                        Options = question.Options.Select(opt => new AttemptOptionViewModel
                        {
                            Id = opt.Id,
                            Text = opt.Text
                        }).ToList()
                    }).ToList()
                };
                return StatusCode(response.StatusCode, viewModel);
            }
           

            return StatusCode(response.StatusCode, response);

        }
    }
}
