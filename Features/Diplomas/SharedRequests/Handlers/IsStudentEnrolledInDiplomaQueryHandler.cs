using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.SharedRequests.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.SharedRequests.Handlers
{
    public class IsStudentEnrolledInDiplomaQueryHandler : IRequestHandler<IsStudentEnrolledInDiplomaQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<StudentEnrollment> _studentEnrollmentRepo;

        public IsStudentEnrolledInDiplomaQueryHandler(IGenericRepository<StudentEnrollment> studentEnrollmentRepo)
        {
            _studentEnrollmentRepo = studentEnrollmentRepo;
        }

        public async Task<RequestResponse<bool>> Handle(IsStudentEnrolledInDiplomaQuery request, CancellationToken cancellationToken)
        {
            var isStudentEnrolled=await _studentEnrollmentRepo.GetAll()
                                                              .AnyAsync(stEnrollment=>
                                                                     stEnrollment.StudentId==request.StudentId&&
                                                                     stEnrollment.DiplomaId==request.DiplomaId,
                                                                     cancellationToken);
            return RequestResponse<bool>.Ok(isStudentEnrolled);
        }
    }
}
