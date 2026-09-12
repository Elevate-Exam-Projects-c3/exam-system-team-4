using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Queries
{
    public record GetSubmittedAndTimedOutAttemptsCountQuery(Guid StudentId, Guid QuizId) : 
                            IRequest<RequestResponse<int>>;
    
    
}
