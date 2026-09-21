using exam_system.Common.Enums;

namespace exam_system.Features.Attempts.SubmitAttempt.ViewModels
{
    public class SubmitAttemptViewModel
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public AttemptStatus AttemptStatus { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public decimal Score { get; set; }
        public bool Passed { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
