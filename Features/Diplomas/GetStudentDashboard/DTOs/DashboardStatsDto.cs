namespace exam_system.Features.Diplomas.GetStudentDashboard.DTOs
{
    public class DashboardStatsDto
    {
        public double AverageScore { get; set; }     // masalan 76.5
        public double PassRate { get; set; }         // nesba me2awya 0-100
        public long TimeSpentSeconds { get; set; }   // ma3'moo3 el wa2t bel sawany
        public int TotalAttempts { get; set; }
    }
}
