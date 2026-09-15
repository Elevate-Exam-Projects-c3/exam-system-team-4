using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public record StudentEnrollDiplomaCommand(Guid DiplomaId, Guid StudentId) : IRequest<RequestResponse<StudentEnrollDiplomaDto>>;
    
}