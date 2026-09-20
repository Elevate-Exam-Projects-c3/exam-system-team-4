namespace exam_system.Features.Attempts.SubmitQuestionAnswer.DTOs
{
    public class SubmitAnswerRequestViewModel
    {
        public Guid QuestionId { get; set; }
        public Guid? SelectedOptionId { get; set; }
    }
}
