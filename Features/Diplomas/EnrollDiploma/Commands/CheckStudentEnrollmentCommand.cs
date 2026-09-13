using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public class CheckStudentEnrollmentCommand : IRequest<bool>
    {
        public Guid StudentId { get; set; }
        public Guid DiplomaId { get; set; }
    }
}