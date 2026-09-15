using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Orchestrators
{
    public record EnrollDiplomaOrchestrator(Guid DiplomaId , Guid StudentId) : IRequest<RequestResponse<bool>>;
   
}