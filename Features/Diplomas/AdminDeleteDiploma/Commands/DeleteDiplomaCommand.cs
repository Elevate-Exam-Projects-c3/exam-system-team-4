using MediatR;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Commands
{
    public record DeleteDiplomaCommand(Guid Id) : IRequest<Unit>;
    
}
