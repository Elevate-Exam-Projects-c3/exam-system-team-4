using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.DTO;
using exam_system.Features.Diplomas.EnrollDiploma.Orchestrators;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class StudentEnrollDiplomaCommandHandler
        : IRequestHandler<StudentEnrollDiplomaCommand, RequestResponse<StudentEnrollDiplomaDto>>
    {
        private readonly IGenericRepository<StudentEnrollment> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public StudentEnrollDiplomaCommandHandler( IGenericRepository<StudentEnrollment> repository, IUnitOfWork unitOfWork  )
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
         
        }

        public async Task<RequestResponse<StudentEnrollDiplomaDto>> Handle( StudentEnrollDiplomaCommand request, CancellationToken cancellationToken)
        {
 
            var enrollment = new StudentEnrollment
            {
                StudentId = request.StudentId,
                DiplomaId = request.DiplomaId,
                EnrolledAt = DateTime.UtcNow
            };

             _repository.Add(enrollment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseDto = new StudentEnrollDiplomaDto
            {
                StudentId = enrollment.StudentId,
                DiplomaId = enrollment.DiplomaId,
                EnrolledAt = enrollment.EnrolledAt
            };

            return RequestResponse<StudentEnrollDiplomaDto>.Created( responseDto, "Student successfully enrolled in the diploma");
        }
    }
}