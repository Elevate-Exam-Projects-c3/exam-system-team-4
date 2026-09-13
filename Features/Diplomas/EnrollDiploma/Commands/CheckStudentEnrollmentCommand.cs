using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public class CheckStudentEnrollmentCommand : IRequest<RequestResponse<bool>>
    {
        public CheckStudentEnrollmentCommand(Guid studentId, Guid diplomaId)
        {
            StudentId = studentId;
            DiplomaId = diplomaId;
        }

        public Guid StudentId { get; set; }
        public Guid DiplomaId { get; set; }
    }
}