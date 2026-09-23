using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Diplomas.GetStudentDashboard.DTOs;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Queries
{
    public record GetMyRecentAttemptsQuery(Guid StudentId) : IRequest<IReadOnlyList<RecentAttemptDto>>;

    public class GetMyRecentAttemptsQueryHandler(
        IGenericRepository<QuizAttempt> attempts) : IRequestHandler<GetMyRecentAttemptsQuery, IReadOnlyList<RecentAttemptDto>>
    {
        private const int Take = 10;

        public async Task<IReadOnlyList<RecentAttemptDto>> Handle(
      GetMyRecentAttemptsQuery request, CancellationToken ct)
        {
            var studentId = request.StudentId;

            return await attempts
                .Get(a => a.StudentId == studentId && a.SubmittedAt != null)
                .AsNoTracking()
                .OrderByDescending(a => a.SubmittedAt)
                .Take(Take)
                .Select(a => new RecentAttemptDto
                {
                    AttemptId = a.Id,
                    QuizTitle = a.Quiz.Title,
                    Score = (decimal)a.Score,
                    Passed = (bool)a.Passed,
                    SubmittedAt = a.SubmittedAt!.Value
                })
                .ToListAsync(ct);
        }
    }
}
