using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.GetAttemptResults.Queries
{
    public record HasInProgressAttemptQuery(Guid QuizId) : IRequest<RequestResponse<bool>>;
}
