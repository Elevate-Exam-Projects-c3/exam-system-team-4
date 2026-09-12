using MediatR;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Commands
{
    public record DeleteQuestionCommand(Guid QuestionId) : IRequest<string>;
    
}
