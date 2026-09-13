using exam_system.Features.Quizzes.AdminPublishQuiz.Commands;
using exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminPublishQuiz.Orchestrators
{

    public record PublishQuizOrchestrator(Guid QuizId) : IRequest<RequestResponse<bool>>;

    public class PublishQuizOrchestratorHandler(IMediator mediator) : IRequestHandler<PublishQuizOrchestrator, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(PublishQuizOrchestrator request, CancellationToken cancellationToken)
        {
            var readinessResult = await  mediator.Send(
            new QuizReadinessQuery(request.QuizId),
            cancellationToken);


            if (!readinessResult.Data.CanPublish)
            {
                return RequestResponse<bool>.Fail("Quiz readiness check failed.", readinessResult.StatusCode, readinessResult.Errors);
            }

            return await mediator.Send(
           new PublishQuizCommand(request.QuizId),
           cancellationToken);
        }
    }
}
