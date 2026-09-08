using exam_system.Features.Diplomas.AdminUpdateDiploma.DTO;
using exam_system.Features.Diplomas.AdminUpdateDiploma.Handlers;
using MediatR;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Commands
{
    public record UpdateDiplomaCommand : IRequest<UpdateDiplomaDto>
    {
        //take the Id
        public Guid Id { get; set; }
        //take the New title and description
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }


        public UpdateDiplomaCommand(Guid id, string title, string? description)
        {
            Id = id;
            Title = title;
            Description = description;
        }

       
    
    }
}
