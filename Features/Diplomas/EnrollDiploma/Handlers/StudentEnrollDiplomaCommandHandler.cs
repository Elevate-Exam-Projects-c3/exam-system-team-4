using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using exam_system.Features.Diplomas.EnrollDiploma.Interfaces;
using exam_system.Features.Diplomas.EnrollDiploma.Orchestrators;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class StudentEnrollDiplomaCommandHandler
        : IRequestHandler<
            StudentEnrollDiplomaCommand,
            RequestResponse<StudentEnrollDiplomaDto>>
    {

        private readonly IEnrollmentOrchestrator _orchestrator;
        private readonly IGenericRepository<StudentEnrollment> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public StudentEnrollDiplomaCommandHandler(IGenericRepository<StudentEnrollment> repository, IUnitOfWork unitOfWork, IEnrollmentOrchestrator orchestrator)
           
        {
            _orchestrator = orchestrator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<StudentEnrollDiplomaDto>> Handle(
            StudentEnrollDiplomaCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Check if student is already enrolled
            var alreadyEnrolled =
                await _orchestrator.IsStudentAlreadyEnrolled(
                    request.StudentId,
                    request.DiplomaId,
                    cancellationToken);

            if (alreadyEnrolled)
            {
                return RequestResponse<StudentEnrollDiplomaDto>.Fail(
                    "Student is already enrolled in this diploma",
                    400);
            }

            // 2. Check if diploma has at least one published quiz
            var hasPublishedQuiz =
                await _orchestrator.HasPublishedQuiz(
                    request.DiplomaId,
                    cancellationToken);

            if (!hasPublishedQuiz)
            {
                return RequestResponse<StudentEnrollDiplomaDto>.Fail(
                    "Diploma is not available for enrollment",
                    400);
            }

            // 3. Create enrollment
            var enrollment = new StudentEnrollment
            {
                StudentId = request.StudentId,
                DiplomaId = request.DiplomaId,
                EnrolledAt = DateTime.UtcNow
            };

             _repository.Add(enrollment);

            // whatever save method you are using
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new StudentEnrollDiplomaDto
            {
                StudentId = request.StudentId,
                DiplomaId = request.DiplomaId,
            };

            return RequestResponse<StudentEnrollDiplomaDto>.Created(
                response,
                "Diploma enrolled successfully");
        }
    }
}