using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitAttempt.Orchestrators
{
    public record SubmitQuizAttemptOrchestrator(Guid attemptId) : IRequest<RequestResponse<SubmitAttemptResponseDto>>;


}
