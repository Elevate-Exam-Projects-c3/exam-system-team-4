using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.StartAttempt.DTOs;
using exam_system.Features.Attempts.StartAttempt.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.StartAttempt.Handlers
{
    public class GetInProgressQuizAttemptQueryHandler : IRequestHandler<GetInProgressQuizAttemptQuery, RequestResponse<StartAttemptResponseDto>>
    {
        private readonly IGenericRepository<QuizAttempt> _quizAttempRepo;

        public GetInProgressQuizAttemptQueryHandler(IGenericRepository<QuizAttempt> quizAttempRepo)
        {
            _quizAttempRepo = quizAttempRepo;
        }

        public async Task<RequestResponse<StartAttemptResponseDto>> Handle(GetInProgressQuizAttemptQuery request, CancellationToken cancellationToken)
        {
            var inProgressAttempt = await _quizAttempRepo.GetAll()
                .Where(attempt => attempt.StudentId == request.StudentId &&
                                attempt.QuizId == request.QuizId &&
                                attempt.Status == AttemptStatus.InProgress)
                .Select(attempt => new StartAttemptResponseDto
                {
                    AttemptId = attempt.Id,
                    StartTime = attempt.StartTime,
                    Deadline = attempt.Deadline

                }).FirstOrDefaultAsync(cancellationToken);
            if (inProgressAttempt is not null)
                return RequestResponse<StartAttemptResponseDto>.Ok(inProgressAttempt);
            else
                return RequestResponse<StartAttemptResponseDto>.Fail(
                    "In-progress attempt not found.",
                    404,
                    new Dictionary<string, string[]>
                    {
                        ["Attempt"] = ["No in-progress attempt was found for this student and quiz."]
                    });



        }
    }
}
