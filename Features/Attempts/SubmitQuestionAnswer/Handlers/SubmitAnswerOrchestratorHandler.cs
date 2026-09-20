using exam_system.Features.Attempts.SubmitQuestionAnswer.Commands;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers
{

    public record SubmitAnswerOrchestrator(
    Guid AttemptId,
    Guid StudentId,
    Guid QuestionId,
    Guid? SelectedOptionId
) : IRequest<RequestResponse<bool>>;


    public class SubmitAnswerOrchestratorHandler(IMediator _mediator) : IRequestHandler<SubmitAnswerOrchestrator, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(SubmitAnswerOrchestrator request, CancellationToken cancellationToken)
        {
            // 1. Validate Student + Attempt + Deadline
            var attemptResult = await _mediator.Send(
                new ValidateAttemptForAnswerQuery(
                    request.AttemptId,
                    request.StudentId),
                cancellationToken);

            if (!attemptResult.Success)
                return attemptResult;


            // 2. Validate Question belongs to Attempt's Quiz
            var questionResult = await _mediator.Send(
                new ValidateQuestionForAttemptQuery(
                    request.AttemptId,
                    request.QuestionId),
                cancellationToken);

            if (!questionResult.Success)
                return questionResult;


            // 3. Validate Option belongs to Question
            
            if (request.SelectedOptionId.HasValue)
            {
                var optionResult = await _mediator.Send(
                    new ValidateQuestionOptionQuery(
                        request.QuestionId,
                        request.SelectedOptionId.Value),
                    cancellationToken);

                if (!optionResult.Success)
                    return optionResult;
            }


            // 4. Save / Update Answer
            return await _mediator.Send(
                new SubmitAnswerCommand(
                    request.AttemptId,
                    request.QuestionId,
                    request.SelectedOptionId),
                cancellationToken);
        }
    }
}
