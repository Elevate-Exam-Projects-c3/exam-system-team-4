using exam_system.Features.Quizzes.Readiness;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries
{
    public record QuizReadinessQuery(Guid QuizId)
     : IRequest<RequestResponse<QuizReadinessDto>>;
}
