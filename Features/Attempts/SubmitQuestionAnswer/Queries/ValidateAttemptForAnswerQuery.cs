using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Queries
{
    public record ValidateAttemptForAnswerQuery(Guid StudentId, Guid AttemptId) : IRequest<RequestResponse<bool>>;
    
}
