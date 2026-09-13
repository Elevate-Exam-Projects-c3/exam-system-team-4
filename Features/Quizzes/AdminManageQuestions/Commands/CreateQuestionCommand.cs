using exam_system.Features.Quizzes.AdminManageQuestions.Dto;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Commands
{


    public record CreateQuestionCommand(
    Guid QuizId,
    string QuestionText,
    string? Explanation,
    int OrderIndex,
    List<QuestionOptionDto> Options) : IRequest<RequestResponse<bool>>;


}
