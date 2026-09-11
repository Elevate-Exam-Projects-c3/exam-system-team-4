using exam_system.Features.Diplomas.AdminUpdateDiploma.DTO;
using exam_system.Features.Diplomas.AdminUpdateDiploma.Handlers;
using MediatR;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Commands
{
    public record UpdateDiplomaCommand(Guid Id, string Title, string? Description) : IRequest<Unit>;
    
}
