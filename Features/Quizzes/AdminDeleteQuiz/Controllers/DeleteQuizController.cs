using exam_system.Features.Quizzes.AdminCreateQuiz.ViewModels;
using exam_system.Features.Quizzes.AdminDeleteQuiz.Commands;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Quizzes.AdminDeleteQuiz.Controllers
{
    [ApiController]
    public class DeleteQuizController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeleteQuizController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpDelete("api/admin/quizzes/{id:Guid}")]
        public async Task<IActionResult> DeleteQuiz(Guid id, CancellationToken cancellationToken)
        {
           var response= await _mediator.Send(new DeleteQuizCommand(id), cancellationToken);
           var endpointResponse= EndpointResponse.FromResult(response);
           return StatusCode(endpointResponse.StatusCode,endpointResponse);
        }
    }
}