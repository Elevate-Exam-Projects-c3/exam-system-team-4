using exam_system.Common.Enums;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using exam_system.Features.Diplomas.EnrollDiploma.Interfaces;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class StudentEnrollDiplomaCommandHandler
     : IRequestHandler<StudentEnrollDiplomaCommand, Unit>
    {
        private readonly IStudentEnrollment _enrollmentRepository;
        private readonly IDiploma _diplomaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<StudentEnrollment> _enrollments;

        public StudentEnrollDiplomaCommandHandler(
            IStudentEnrollment enrollmentRepository,
            IDiploma diplomaRepository,
            IUnitOfWork unitOfWork , IGenericRepository<StudentEnrollment> enrollments)
        {
            _enrollmentRepository = enrollmentRepository;
            _diplomaRepository = diplomaRepository;
            _unitOfWork = unitOfWork;
            _enrollments = enrollments;
        }

        public async Task<Unit> Handle( StudentEnrollDiplomaCommand request, CancellationToken cancellationToken)
        {
            // Check duplicate enrollment
            var alreadyEnrolled = await _enrollmentRepository.EnrolledBefore( request.StudentId, request.DiplomaId);

            if (alreadyEnrolled = true)
            {
                throw new Exception( "Student is already enrolled in this diploma");
            }

            // Check published quizzes
            var hasPublishedQuiz = await _diplomaRepository.HasPublishedQuizAsync( request.DiplomaId, cancellationToken);

            if (!hasPublishedQuiz)
            {
                throw new Exception( "Diploma is not available for enrollment");
            }

            // Create enrollment
            var enrollment = new StudentEnrollment
            {
                StudentId = request.StudentId,
                DiplomaId = request.DiplomaId,
                EnrolledAt = DateTime.UtcNow
            };

             _enrollments.Add(enrollment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

