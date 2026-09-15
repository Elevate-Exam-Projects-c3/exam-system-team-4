using exam_system.Domain.Entities.Quizzes;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Dto
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public int OrderIndex { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = null!;
    }
}
