using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public record CheckStudentEnrollmentCommand(Guid studentId, Guid diplomaId) : IRequest<RequestResponse<bool>>;
   
}