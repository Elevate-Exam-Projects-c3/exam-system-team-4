using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Diplomas.BrowseDiplomas.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.BrowseDiplomas.Queries
{
    public record BrowseDiplomasQuery(Guid? StudentID , int PageSize = 5 , int PageNumber = 1) : IRequest<RequestResponse<PaginatedResult<BrowseDiplomaDto>>>;
    
}
