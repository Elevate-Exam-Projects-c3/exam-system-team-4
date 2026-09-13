namespace exam_system.Features.Diplomas.EnrollDiploma.Orchestrators
{
    public interface IEnrollmentOrchestrator
    {
        Task<bool> IsStudentAlreadyEnrolled(
            Guid studentId,
            Guid diplomaId,
            CancellationToken cancellationToken);

        Task<bool> HasPublishedQuiz(
            Guid diplomaId,
            CancellationToken cancellationToken);
    }
}