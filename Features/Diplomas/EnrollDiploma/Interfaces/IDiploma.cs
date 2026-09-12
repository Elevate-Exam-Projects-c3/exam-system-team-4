namespace exam_system.Features.Diplomas.EnrollDiploma.Interfaces
{
    public interface IDiploma
    {
        Task<bool> HasPublishedQuizAsync( Guid diplomaId, CancellationToken cancellationToken);
    }
}
