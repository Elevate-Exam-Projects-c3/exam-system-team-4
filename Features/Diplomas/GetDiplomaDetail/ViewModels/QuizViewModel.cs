using exam_system.Domain.Entities.Attempts;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.ViewModels
{
    public class QuizViewModel
    {
        public Guid QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int PassScore { get; set; } = 60;
        public int? MaxAttempts { get; set; }
        public int AttemptsUsed { get; set; }
        public bool HasInProgressAttempt { get; set; }
        public bool IsResumable { get; set; }
        public List<AttemptViewModel> Attempts { get; set; } = new();
    }
}
