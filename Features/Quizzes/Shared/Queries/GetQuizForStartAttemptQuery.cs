using exam_system.Features.Quizzes.Shared.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.Shared.Queries
{
    public record GetQuizForStartAttemptQuery(Guid Id) : IRequest<RequestResponse<QuizStartAttemptDto>>;
    
}
