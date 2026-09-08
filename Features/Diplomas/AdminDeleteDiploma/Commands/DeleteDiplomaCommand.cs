using MediatR;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Commands
{
    public record DeleteDiplomaCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
     
    }
}
