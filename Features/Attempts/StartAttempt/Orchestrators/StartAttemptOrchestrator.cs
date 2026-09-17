using exam_system.Features.Attempts.StartAttempt.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Orchestrators
{
    public record StartAttemptOrchestrator(Guid QuizId) :
        IRequest<RequestResponse<StartAttemptResponseDto>>;
    
}
