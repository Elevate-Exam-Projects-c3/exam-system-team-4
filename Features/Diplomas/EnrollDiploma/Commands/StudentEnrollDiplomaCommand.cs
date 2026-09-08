using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public class StudentEnrollDiplomaCommand : IRequest<StudentEnrollDiplomaDto>
    {
        public Guid DiplomaId { get; set; }
    }
}
