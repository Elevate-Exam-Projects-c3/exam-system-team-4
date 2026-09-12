namespace exam_system.Features.Quizzes.AdminManageQuestions.Dto
{
    public class QuestionOptionDto
    {
        public Guid Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
