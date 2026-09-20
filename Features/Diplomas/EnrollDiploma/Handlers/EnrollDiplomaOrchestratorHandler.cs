using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.Orchestrators;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class EnrollDiplomaOrchestratorHandler : IRequestHandler<EnrollDiplomaOrchestrator, RequestResponse<bool>>
    {
        private readonly IMediator _mediator;

        public EnrollDiplomaOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<bool>> Handle(EnrollDiplomaOrchestrator request, CancellationToken cancellationToken)
        {
            var isAlreadyEnrolledResult = await _mediator.Send(new CheckStudentEnrollmentCommand(request.StudentId, request.DiplomaId), cancellationToken);

            if (isAlreadyEnrolledResult.Data)
            {
                return RequestResponse<bool>.Fail("Student already enrolled in this diploma before");
            }

            var hasPublishedQuizResult = await _mediator.Send( new CheckPublishedQuizCommand(request.DiplomaId), cancellationToken);


            if (!hasPublishedQuizResult.Data)
            {

                return RequestResponse<bool>.Fail("Diploma is not available");
            }

            var enrollCommandResult = await _mediator.Send(new StudentEnrollDiplomaCommand(request.StudentId, request.DiplomaId), cancellationToken);

            if (!enrollCommandResult.Success)
            {
                return RequestResponse<bool>.Fail(enrollCommandResult.Message, enrollCommandResult.StatusCode);
            }

            return RequestResponse<bool>.Created(true, enrollCommandResult.Message);
        }
    }
}
