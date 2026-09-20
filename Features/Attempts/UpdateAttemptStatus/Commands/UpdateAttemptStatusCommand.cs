using exam_system.Common.Enums;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.UpdateAttemptStatus.Commands
{
    public record UpdateAttemptStatusCommand(Guid Id, AttemptStatus newStatus) : IRequest<RequestResponse>;

}
