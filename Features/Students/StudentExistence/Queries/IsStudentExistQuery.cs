using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Students.StudentExistence.Queries
{
    public record IsStudentExistQuery(Guid Id) : IRequest<RequestResponse<bool>>;
    
}
