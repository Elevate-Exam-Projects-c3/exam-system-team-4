using exam_system.Features.Attempts.StartAttempt.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Queries
{
    public record GetInProgressQuizAttemptQuery(
        Guid StudentId ,
        Guid QuizId) : IRequest<RequestResponse<StartAttemptResponseDto>>;
    
}
