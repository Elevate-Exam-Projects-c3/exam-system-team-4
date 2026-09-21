using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitAttempt.Queries
{
    public record GetAttemptScoreQuery(Guid AttemptId,Guid QuizId)
    : IRequest<RequestResponse<AttemptScoreDto>>;
}
