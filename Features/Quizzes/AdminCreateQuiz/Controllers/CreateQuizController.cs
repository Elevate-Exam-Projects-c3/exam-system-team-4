using Azure;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Quizzes.AdminCreateQuiz.ViewModels;
using exam_system.Features.Shared;
using exam_system.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Controllers
{
    [ApiController]
    [Authorize(Roles = SD.Admin)]
    public class CreateQuizController: ControllerBase
    {
        private readonly IMediator _mediator;

        public CreateQuizController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("api/admin/quizzes")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizViewModel viewModel,CancellationToken cancellationToken)
        {
            var requestResult=await _mediator.Send(new CreateQuizCommand(
                DiplomaId: viewModel.DiplomaId,
                Title: viewModel.Title,
                DurationMinutes:viewModel.DurationMinutes,
                Instructions:viewModel.Instructions,
                StartDate:viewModel.StartDate,
                EndDate:viewModel.EndDate,
                PassScore: viewModel.PassScore,
                MaxAttempts:viewModel.MaxAttempts                                                             
                ), cancellationToken);
            return StatusCode(requestResult.StatusCode, requestResult);
        }
    }
}
