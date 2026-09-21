using exam_system.Common.Enums;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.GetDiplomaDetail.DTOs;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.Queries
{
    public record GetDiplomaQuery(Guid DiplomaId, Guid StudentId) : IRequest<RequestResponse<GetDiplomaDto>>;


    public class GetDiplomaQueryHandler : IRequestHandler<GetDiplomaQuery, RequestResponse<GetDiplomaDto>>
    {

        private readonly IGenericRepository<Diploma> _repository;

        public GetDiplomaQueryHandler(IGenericRepository<Diploma> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResponse<GetDiplomaDto>> Handle(GetDiplomaQuery request, CancellationToken ct)
        {


            var sid = request.StudentId;

            var diploma = await _repository.GetAll()
                .AsNoTracking()
                .Where(d => d.Id == request.DiplomaId && !d.IsDeleted)   
                .Select(d => new GetDiplomaDto
                {
                    DiplomaId = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    Quizzes = d.Quizzes
                        .Where(q => q.Status == QuizStatus.Published)
                        .Select(q => new QuizDto
                        {
                            QuizId = q.Id,
                            Title = q.Title,
                            Duration = q.DurationMinutes,
                            PassScore = q.PassScore,
                            MaxAttempts = q.MaxAttempts,
                            AttemptsUsed = q.Attempts.Count(a => a.StudentId == sid && a.Status != AttemptStatus.InProgress),
                            HasInProgressAttempt = q.Attempts.Any(a => a.StudentId == sid && a.Status == AttemptStatus.InProgress),
                            Attempts = q.Attempts
                             .Where(a => a.StudentId == sid)
                                 .Select(a => new AttemptDto
                                         {
                                           AttemptId = a.Id,
                                             QuizId = a.QuizId,
                                             StartTime = a.StartTime,
                                                 Deadline = a.Deadline,
                                                 ShuffleSeed = a.ShuffleSeed,
                                           LastAnsweredQuestionId = a.LastAnsweredQuestionId
                                                           })
                                                          .ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (diploma is null || diploma.Quizzes.Count == 0)
                return RequestResponse<GetDiplomaDto>.Fail("Diploma not found", 404); 

            foreach (var q in diploma.Quizzes)
            {
                q.IsResumable = q.HasInProgressAttempt;
                q.CanAttempt = !q.HasInProgressAttempt && (q.MaxAttempts == null || q.AttemptsUsed < q.MaxAttempts);
            }

            return RequestResponse<GetDiplomaDto>.Ok(diploma, "Diploma Retrieved Successfully", 200);
        }
    }
}
