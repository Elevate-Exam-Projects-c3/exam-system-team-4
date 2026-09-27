namespace exam_system.Features.Diplomas.GetStudentDashboard.DTOs
{
    public class RecentAttemptDto
    {
        public Guid AttemptId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public bool Passed { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
