using exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Orchestrators
{
    public record SubmitAnswerOrchestrator(Guid StudentId,Guid AttemptId, Guid QuestionId, Guid? SelectedOptionId) : IRequest<RequestResponse<bool>>;

    public class SubmitAnswerOrchestratorHnadler(IMediator mediator) : IRequestHandler<SubmitAnswerOrchestrator, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(SubmitAnswerOrchestrator request, CancellationToken cancellationToken)
        {
            var validateAttemptForAnswery = await mediator.Send(new ValidateAttemptForAnswerQuery(request.StudentId, request.AttemptId), cancellationToken);


            if (!validateAttemptForAnswery.Data)
            {
                return RequestResponse<bool>.Fail(validateAttemptForAnswery.Message, validateAttemptForAnswery.StatusCode, validateAttemptForAnswery.Errors);
            }

            return RequestResponse<bool>.Ok(true);
        }
    }
}
