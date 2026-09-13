using exam_system.Features.Attempts.GetAttemptResults.Queries;
using exam_system.Features.Quizzes.AdminPublishQuiz.Commands;
using exam_system.Features.Shared;
using MediatR;
using System.Net;

namespace exam_system.Features.Quizzes.AdminPublishQuiz.Orchestrators
{

    public record UnpublishQuizOrchestrator(Guid QuizId) : IRequest<RequestResponse<bool>>;

    public class UnpublishQuizOrchestratorHandler(IMediator mediator) : IRequestHandler<UnpublishQuizOrchestrator, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(UnpublishQuizOrchestrator request, CancellationToken cancellationToken)
        {
            var hasInProgressAttempt = await mediator.Send(
          new HasInProgressAttemptQuery(request.QuizId),
          cancellationToken);

            if (hasInProgressAttempt.Data)
            {
                return RequestResponse<bool>.Fail("quiz has In Progress Attempt..unpublishing not allowed", (int)HttpStatusCode.Conflict);
            }

            return await mediator.Send(
                new UnpublishQuizCommand(request.QuizId),
                cancellationToken);
        }
    }
}
