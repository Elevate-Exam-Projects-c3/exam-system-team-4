using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.SharedRequests.Queries
{
    public record IsStudentEnrolledInDiplomaQuery(Guid StudentId,Guid DiplomaId):
        IRequest<RequestResponse<bool>>;
    
}
