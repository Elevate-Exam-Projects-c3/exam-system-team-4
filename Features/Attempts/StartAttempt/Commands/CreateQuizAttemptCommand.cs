using exam_system.Common.Enums;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Commands
{
    public record CreateQuizAttemptCommand(Guid StudentId,
                                           Guid QuizId,
                                           DateTime StartTime,
                                           DateTime Deadline,
                                           AttemptStatus attemptStatus
                                           ) : IRequest<RequestResponse>;
    
}
