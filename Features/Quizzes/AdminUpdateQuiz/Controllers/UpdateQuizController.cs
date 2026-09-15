using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using exam_system.Features.Quizzes.AdminUpdateQuiz.ViewModels;
using exam_system.Helper;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Quizzes.AdminUpdateQuiz.Controllers
{
    [ApiController]
    [Authorize(Roles = SD.Admin)]
    public class UpdateQuizController:ControllerBase
    {
        private readonly IMediator _mediator;

        public UpdateQuizController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPut("api/admin/quizzes/{id:Guid}")]
        public async Task<IActionResult> UpdateQuiz(Guid id ,[FromBody] UpdateQuizViewModel viewModel, CancellationToken cancellationToken)
        {
            var requestResult = await _mediator.Send(new UpdateQuizCommand(Id:id,
                Title: viewModel.Title,
                DurationMinutes: viewModel.DurationMinutes,
                Instructions: viewModel.Instructions,
                PassScore: viewModel.PassScore,
                StartDate:viewModel.StartDate,
                EndDate:viewModel.EndDate,
                MaxAttempts: viewModel.MaxAttempts
                ), cancellationToken);
            return StatusCode(requestResult.StatusCode, requestResult);
        }
    }
}
