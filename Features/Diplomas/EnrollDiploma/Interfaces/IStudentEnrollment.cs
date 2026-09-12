using exam_system.Domain.Entities.Diplomas;

namespace exam_system.Features.Diplomas.EnrollDiploma.Interfaces
{
    public interface IStudentEnrollment
    {
        //does this student enrolled before in this diploma
        Task<bool> EnrolledBefore( Guid studentId, Guid diplomaId);
       
    }
}
