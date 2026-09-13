using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Orchestrators
{
    public class EnrollDiplomaOrchestrator
    {
        private readonly IMediator _mediator;

        public EnrollDiplomaOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<bool>> ExecuteAsync(  Guid studentId,Guid diplomaId,  CancellationToken cancellationToken = default)
        {
            var isAlreadyEnrolledResult = await _mediator.Send(new CheckStudentEnrollmentCommand(studentId, diplomaId),  cancellationToken);

            if (!isAlreadyEnrolledResult.Success)
            {
                return RequestResponse<bool>.Fail(isAlreadyEnrolledResult.Message);
            }

            if (isAlreadyEnrolledResult.Data)
            {
                return RequestResponse<bool>.Fail("Student already enrolled in this diploma before");
            }

            var hasPublishedQuizResult = await _mediator.Send(
                new CheckPublishedQuizCommand(diplomaId),
                cancellationToken);

            if (!hasPublishedQuizResult.Success)
            {
                return RequestResponse<bool>.Fail(hasPublishedQuizResult.Message);
            }

            if (hasPublishedQuizResult.Data)
            {
                
                return RequestResponse<bool>.Fail("Diploma is not available");
            }

            var enrollCommandResult = await _mediator.Send( new StudentEnrollDiplomaCommand(studentId, diplomaId),  cancellationToken);

            if (!enrollCommandResult.Success)
            {
                return RequestResponse<bool>.Fail(enrollCommandResult.Message, enrollCommandResult.StatusCode);
            }

            return RequestResponse<bool>.Created(true, enrollCommandResult.Message);
        }
    }
}