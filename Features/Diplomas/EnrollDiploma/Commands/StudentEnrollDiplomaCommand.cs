using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public record StudentEnrollDiplomaCommand(Guid DiplomaId, Guid StudentId) : IRequest<Unit>;
    
}