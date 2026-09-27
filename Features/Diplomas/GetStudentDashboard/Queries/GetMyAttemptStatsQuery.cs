using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Diplomas.GetStudentDashboard.DTOs;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Queries
{
    public record GetMyAttemptStatsQuery(Guid StudentId) : IRequest<DashboardStatsDto>;

    public class GetMyAttemptStatsQueryHandler( IGenericRepository<QuizAttempt> attempts) : IRequestHandler<GetMyAttemptStatsQuery,DashboardStatsDto>
    {
        public async Task<DashboardStatsDto> Handle(
            GetMyAttemptStatsQuery request, CancellationToken ct)
        {
            var studentId = request.StudentId;

            var raw = await attempts
                .Get(a => a.StudentId == studentId && a.SubmittedAt != null)
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Passed = g.Count(a => (bool)a.Passed),
                    Average = g.Average(a => a.Score),
                    Seconds = g.Sum(a => (long)EF.Functions.DateDiffSecond(
                                  a.StartTime, a.SubmittedAt!.Value))
                })
                .FirstOrDefaultAsync(ct);

            // Student gdid: mafeesh attempts, fa nrga3 0 msh error
            return raw is null ? new DashboardStatsDto() : new DashboardStatsDto
     {
         AverageScore = Math.Round((double)raw.Average, 2),
         PassRate = Math.Round(raw.Passed * 100.0 / raw.Total, 2),
         TimeSpentSeconds = raw.Seconds,
         TotalAttempts = raw.Total
     };
        }
    }
}
