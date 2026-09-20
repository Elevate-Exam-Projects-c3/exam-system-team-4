using exam_system.Common.Enums;
using exam_system.Features.Attempts.UpdateAttemptStatus.Commands;
using MediatR;

namespace exam_system.Features.Jobs.Attempts
{
    public class TimedOutQuizAttemptJob
    {
        private readonly IMediator _mediator;

        public TimedOutQuizAttemptJob(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Execute(Guid attemptId)
        {
            await _mediator.Send(
                new UpdateAttemptStatusCommand(attemptId, AttemptStatus.TimedOut));
        }

    }
}
