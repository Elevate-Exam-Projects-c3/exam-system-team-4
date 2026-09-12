using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Queries
{
    public record QuizExistsQuery(Guid QuizId) : IRequest<bool>;
}
