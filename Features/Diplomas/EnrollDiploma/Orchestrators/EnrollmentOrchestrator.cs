using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Orchestrators
{
    public class EnrollmentOrchestrator : IEnrollmentOrchestrator
    {
        private readonly IMediator _mediator;

        public EnrollmentOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> IsStudentAlreadyEnrolled(
            Guid studentId,
            Guid diplomaId,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(
                new CheckStudentEnrollmentCommand
                {
                    StudentId = studentId,
                    DiplomaId = diplomaId
                },
                cancellationToken);
        }

        public async Task<bool> HasPublishedQuiz(
            Guid diplomaId,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(
                new CheckPublishedQuizCommand
                {
                    DiplomaId = diplomaId
                },
                cancellationToken);
        }
    }
}