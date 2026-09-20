using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Queries
{
    public record ValidateQuestionForAttemptQuery(
    Guid AttemptId,
    Guid QuestionId
) : IRequest<RequestResponse<bool>>;
}
