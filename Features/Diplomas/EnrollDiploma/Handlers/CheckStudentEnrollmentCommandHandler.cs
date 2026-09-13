using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.Interfaces;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class CheckStudentEnrollmentCommandHandler
        : IRequestHandler<CheckStudentEnrollmentCommand, bool>
    {
        private readonly IStudentEnrollment _enrollmentRepository;

        public CheckStudentEnrollmentCommandHandler(
            IStudentEnrollment enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<bool> Handle(
            CheckStudentEnrollmentCommand request,
            CancellationToken cancellationToken)
        {
            return await _enrollmentRepository.EnrolledBefore(
                request.StudentId,
                request.DiplomaId);
        }
    }
}