namespace exam_system.Features.Diplomas.GetStudentDashboard.DTOs
{
    public class StudentDashboardDto
    {
        public List<EnrolledDiplomaDto> EnrolledDiplomas { get; set; } = [];
        public List<RecentAttemptDto> RecentAttempts { get; set; } = [];
        public DashboardStatsDto Stats { get; set; } = new();
    }
}
