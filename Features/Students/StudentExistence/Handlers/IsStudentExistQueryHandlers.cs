using exam_system.Domain.Entities.Identity;
using exam_system.Features.Shared;
using exam_system.Features.Students.StudentExistence.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Students.StudentExistence.Handlers
{
    public class IsStudentExistQueryHandlers : IRequestHandler<IsStudentExistQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Student> _studentRepo;

        public IsStudentExistQueryHandlers(IGenericRepository<Student> studentRepo)
        {
            _studentRepo = studentRepo;
        }

        public async Task<RequestResponse<bool>> Handle(IsStudentExistQuery request, CancellationToken cancellationToken)
        {
            var isStudentExist = await _studentRepo.GetAll().AnyAsync(st => st.Id == request.Id,
                                            cancellationToken);
            return RequestResponse<bool>.Ok(isStudentExist);
                }
    }
}
