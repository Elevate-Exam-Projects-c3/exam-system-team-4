namespace exam_system.Features.Diplomas.GetDiplomaDetail.ViewModels
{
    public class ViewDiplomaDetailsWithQuizzesViewModel
    {
        public Guid DiplomaId { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<QuizViewModel>? Quizzes { get; set; }
    }
}
