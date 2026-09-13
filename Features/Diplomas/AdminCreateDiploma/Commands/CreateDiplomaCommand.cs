using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminCreateDiploma.DTO;
using MediatR;

namespace exam_system.Features.Diplomas.AdminCreateDiploma.Commands
{
    //create "request" for creating a diploma returns DTO as a response
    public record CreateDiplomaCommand(string Title, string? Description) : IRequest<Unit>;
   
}
