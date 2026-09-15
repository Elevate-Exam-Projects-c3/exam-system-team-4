using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class CheckStudentEnrollmentCommandHandler
        : IRequestHandler<CheckStudentEnrollmentCommand, RequestResponse<bool>>
    {
        private readonly IGenericRepository<StudentEnrollment> _repository;

        public CheckStudentEnrollmentCommandHandler(IGenericRepository<StudentEnrollment> repository)
        {
            _repository = repository;
        }

        public async Task<RequestResponse<bool>> Handle(CheckStudentEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var isEnrolled = await _repository.GetAll()
                .AnyAsync(sd => sd.StudentId == request.studentId
                             && sd.DiplomaId == request.diplomaId, cancellationToken);

            return RequestResponse<bool>.Ok(isEnrolled);
        }
    }
}