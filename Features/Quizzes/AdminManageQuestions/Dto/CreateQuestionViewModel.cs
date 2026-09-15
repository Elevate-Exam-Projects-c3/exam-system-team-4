namespace exam_system.Features.Quizzes.AdminManageQuestions.Dto
{
    public record CreateQuestionViewModel(
  Guid QuizId,
  string QuestionText,
  string? Explanation,
  int OrderIndex,
  List<QuestionOptionDto> Options);
}
