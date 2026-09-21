using exam_system.Common.Enums;

namespace exam_system.Features.Attempts.SubmitAttempt.DTOs
{
    public class QuizAttemptForSubmissionDto
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public Guid StudentId { get; set; }

        public AttemptStatus Status { get; set; }
        public DateTime Deadline { get; set; }

        public decimal PassScore { get; set; }
        public int CorrectAnswersCount { get; set; }
    }
}
