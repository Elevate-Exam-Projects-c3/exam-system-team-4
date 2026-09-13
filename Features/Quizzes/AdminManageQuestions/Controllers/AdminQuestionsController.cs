using exam_system.Features.Quizzes.AdminManageQuestions.Commands;
using exam_system.Features.Quizzes.AdminManageQuestions.Dto;
using exam_system.Features.Quizzes.AdminManageQuestions.Orchestrators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Admin")]
    public class AdminQuestionsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;


        [HttpPost]
        public async Task<IActionResult> Create(
            CreateQuestionViewModel model,
            CancellationToken cancellationToken)
        {
            var  command = new CreateQuestionOrchestrator(
                model.QuizId,
                model.QuestionText,
                model.Explanation,
                model.OrderIndex,
                model.Options);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{questionId:guid}")]
        public async Task<IActionResult> DeleteQuestion(
        Guid questionId,
        CancellationToken cancellationToken)
        {
           var result = await _mediator.Send(
                new DeleteQuestionCommand(questionId),
                cancellationToken);

            return Ok(result);
        }
    }
}
