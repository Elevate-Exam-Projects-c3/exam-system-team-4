using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SharedRequests.Queries
{
    public record GetInProgressQuizAttemptsCountQuery(Guid quizId) : IRequest<RequestResponse<int>>;
    
}
