using Azure;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Quizzes.AdminCreateQuiz.ViewModels;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Controllers
{
    [ApiController]
    public class CreateQuizController: ControllerBase
    {
        private readonly IMediator _mediator;

        public CreateQuizController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizViewModel viewModel,CancellationToken cancellationToken)
        {
            var requestResult=await _mediator.Send(new CreateQuizCommand(
                                                               DiplomaId: viewModel.DiplomaId,
                                                                Title: viewModel.Title,
                                                                DurationMinutes:viewModel.DurationMinutes,
                                                                Instructions:viewModel.Instructions,
                                                                PassScore: viewModel.PassScore,
                                                                MaxAttempts:viewModel.MaxAttempts                                                             
                                                                ), cancellationToken);
            return StatusCode(requestResult.StatusCode, requestResult);
        }
    }
}
