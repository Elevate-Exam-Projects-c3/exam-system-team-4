using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.GetStudentDashboard.DTOs;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Queries
{
    public record GetMyEnrolledDiplomasQuery(Guid StudentId) : IRequest<IReadOnlyList<EnrolledDiplomaDto>>;

    public class GetMyEnrolledDiplomasQueryHandler( IGenericRepository<StudentEnrollment> enrollments) : IRequestHandler<GetMyEnrolledDiplomasQuery, IReadOnlyList<EnrolledDiplomaDto>>
    {
        public async Task<IReadOnlyList<EnrolledDiplomaDto>> Handle( GetMyEnrolledDiplomasQuery request, CancellationToken ct)
        {
            var studentId = request.StudentId;

            return await enrollments
                .Get(e => e.StudentId == studentId)
                .AsNoTracking()
                .Select(e => new EnrolledDiplomaDto
                {
                    Id = e.Diploma.Id,
                    Title = e.Diploma.Title,
                    EnrolledAt = e.EnrolledAt
                })
                .ToListAsync(ct);
        }
    }
}
