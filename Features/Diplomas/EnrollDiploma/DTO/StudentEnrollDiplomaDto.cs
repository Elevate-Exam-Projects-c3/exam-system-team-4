namespace exam_system.Features.Diplomas.EnrollDiploma.DTO
{
    public class StudentEnrollDiplomaDto
    {
        public Guid EnrollmentId { get; set; }
        public Guid DiplomaId { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
