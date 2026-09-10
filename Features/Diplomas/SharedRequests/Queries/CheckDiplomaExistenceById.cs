using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.SharedRequests.Queries
{
    public record CheckDiplomaExistenceById(Guid diplomaId) : IRequest<RequestResponse<bool>>;

}
