using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminCreateDiploma.DTO;
using MediatR;

namespace exam_system.Features.Diplomas.AdminCreateDiploma.Commands
{
    //create "request" for creating a diploma returns DTO as a response
    public record CreateDiplomaCommand : IRequest<CreateDiplomaDto>
    {
        //takes the title and description of the diploma as parameters
        public string Title { get; set;  } = string.Empty;
       public string? Description { get; set; }

       
    }
}
