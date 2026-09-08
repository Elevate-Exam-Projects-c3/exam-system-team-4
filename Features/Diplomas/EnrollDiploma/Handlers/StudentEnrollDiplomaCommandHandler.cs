using exam_system.Common.Enums;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class StudentEnrollDiplomaCommandHandler
    : IRequestHandler<StudentEnrollDiplomaCommand, StudentEnrollDiplomaDto>
    {
        private readonly IGenericRepository<Diploma> _diplomaRepository;
        private readonly IGenericRepository<StudentEnrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StudentEnrollDiplomaCommandHandler(
            IGenericRepository<Diploma> diplomaRepository,
            IGenericRepository<StudentEnrollment> enrollmentRepository,
            IUnitOfWork unitOfWork)
        {
            _diplomaRepository = diplomaRepository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentEnrollDiplomaDto> Handle( StudentEnrollDiplomaCommand request,CancellationToken cancellationToken)
        {
            // 1. Get diploma
            var diploma = await _diplomaRepository.GetByIdAsync(request.DiplomaId, cancellationToken);

            // 2. Check diploma exists
            if (diploma == null)
            {
                throw new KeyNotFoundException("Diploma not found.");
            }

            // 3. Check published quizzes
            if (!diploma.Quizzes.Any(q => q.Status == QuizStatus.Published))
            {
                throw new InvalidOperationException(
                    "No published quizzes found for the specified diploma.");
            }


            // 5. Create enrollment
            var enrollment = new StudentEnrollment
            {
                DiplomaId = request.DiplomaId,
                EnrolledAt = DateTime.UtcNow
            };

            // 6. Add enrollment
             _enrollmentRepository.Add(enrollment);

            // 7. Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 8. Return DTO
            return new StudentEnrollDiplomaDto
            {
                EnrollmentId = enrollment.Id,
                DiplomaId = enrollment.DiplomaId,
                EnrolledAt = enrollment.EnrolledAt
            };
        }
    }
}
