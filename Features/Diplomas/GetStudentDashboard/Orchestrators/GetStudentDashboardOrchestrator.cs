using exam_system.Domain.Entities.Identity;
using exam_system.Features.Diplomas.GetStudentDashboard.DTOs;
using exam_system.Features.Diplomas.GetStudentDashboard.Queries;
using MediatR;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Orchestrators
{
    public record GetStudentDashboardOrchestrator(Guid StudentId) : IRequest<StudentDashboardDto>;

    public class GetStudentDashboardQueryHandler(IMediator mediator)  : IRequestHandler<GetStudentDashboardOrchestrator, StudentDashboardDto>
    {
        public async Task<StudentDashboardDto> Handle(GetStudentDashboardOrchestrator request, CancellationToken ct)
        {
            // Sequential 3amdan: kolohom bey-share nafs el DbContext (Scoped)
            var diplomas = await mediator.Send(new GetMyEnrolledDiplomasQuery(request.StudentId));
            var recent = await mediator.Send(new GetMyRecentAttemptsQuery(request.StudentId));
            var stats = await mediator.Send(new GetMyAttemptStatsQuery(request.StudentId));

            return new StudentDashboardDto
            {
                EnrolledDiplomas = diplomas.ToList(),
                RecentAttempts = recent.ToList(),
                Stats = stats
            };
        }
    }
}
