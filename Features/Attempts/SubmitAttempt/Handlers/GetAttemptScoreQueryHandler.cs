using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Attempts.SubmitAttempt.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.SubmitAttempt.Handlers
{
    public class GetAttemptScoreQueryHandler
    : IRequestHandler<GetAttemptScoreQuery, RequestResponse<AttemptScoreDto>>
    {
        private readonly IGenericRepository<Question> _questionRepo;

        public GetAttemptScoreQueryHandler(IGenericRepository<Question> questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<RequestResponse<AttemptScoreDto>> Handle(
            GetAttemptScoreQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _questionRepo.GetAll()
                                        .AsNoTracking()
                                        .Where(q => q.QuizId == request.QuizId)
                                        .GroupBy(q => q.QuizId)
                                        .Select(g => new AttemptScoreDto
                                        {
                                            TotalQuestions = g.Count(),

                                            CorrectAnswers = g
                                                .SelectMany(q => q.Answers)
                                                .Count(studentAnswer =>
                                                    studentAnswer.AttemptId == request.AttemptId &&
                                                    studentAnswer.SelectedOptionId.HasValue &&
                                                    studentAnswer.Question.Options.Any(option =>
                                                        option.Id == studentAnswer.SelectedOptionId &&
                                                        option.IsCorrect))
                                        })

                                        .FirstOrDefaultAsync(cancellationToken);
            if (result is null)
            {
                return RequestResponse<AttemptScoreDto>
                    .Fail("Attempt not found.");
            }

            return RequestResponse<AttemptScoreDto>.Ok(result);
        }
    }
}
