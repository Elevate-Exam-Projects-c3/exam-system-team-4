using exam_system.Features.Quizzes.AdminCreateQuiz.Queries;
using exam_system.Features.Quizzes.AdminManageQuestions.Commands;
using exam_system.Features.Quizzes.AdminManageQuestions.Dto;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Orchestrators
{

    public record CreateQuestionOrchestrator(
    Guid QuizId,
    string QuestionText,
    string? Explanation,
    int OrderIndex,
    List<QuestionOptionDto> Options) : IRequest<RequestResponse<bool>>;

    public class CreateQuestionOrchestratorHandler : IRequestHandler<CreateQuestionOrchestrator, RequestResponse<bool>>
    {
        private readonly IMediator _mediator;

        public CreateQuestionOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<bool>> Handle(CreateQuestionOrchestrator request, CancellationToken cancellationToken)
        {
            var quizExists = await _mediator.Send(
           new QuizExistsQuery(request.QuizId),
           cancellationToken);

            if (!quizExists)
                throw new KeyNotFoundException("Quiz not found.");


            var command = new CreateQuestionCommand(
                request.QuizId,
                request.QuestionText,
                request.Explanation,
                request.OrderIndex,
                request.Options
            );

            return await _mediator.Send(
        command,
        cancellationToken);
        }
    }
}
