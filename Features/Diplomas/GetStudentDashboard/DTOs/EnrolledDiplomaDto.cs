namespace exam_system.Features.Diplomas.GetStudentDashboard.DTOs
{
    public class EnrolledDiplomaDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
    }
}
