using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries;
using exam_system.Features.Quizzes.Readiness;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Handlers
{
    public class QuizReadinessQueryHandler(IGenericRepository<Quiz> quizRepository) : IRequestHandler<QuizReadinessQuery, RequestResponse<QuizReadinessDto>>
    {
        public async Task<RequestResponse<QuizReadinessDto>> Handle(QuizReadinessQuery request, CancellationToken cancellationToken)
        {
            var quiz = quizRepository.GetAll()
                .Where(q => q.Id == request.QuizId)
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefault();

            if (quiz == null)
            {
                throw new KeyNotFoundException(
                    $"Quiz with id '{request.QuizId}' was not found.");
            }

            var checks = new List<ReadinessCheckDto>();


            var hasQuestions = quiz.Questions.Any();

            checks.Add(new ReadinessCheckDto
            {
                Name = "HasQuestions",

                Passed = hasQuestions,

                Message = hasQuestions
                ? "Quiz has at least one question."
                : "Quiz must have at least one question."
            });

            var questionsHaveOneCorrectOption =
           quiz.Questions.All(question =>
               question.Options.Count(option => option.IsCorrect) == 1
           );

            checks.Add(new ReadinessCheckDto
            {
                Name = "QuestionsHaveExactlyOneCorrectOption",

                Passed = questionsHaveOneCorrectOption,

                Message = questionsHaveOneCorrectOption
               ? "Every question has exactly one correct option."
               : "Every question must have exactly one correct option."
            });

            var validDuration =
          quiz.DurationMinutes > 0;

            checks.Add(new ReadinessCheckDto
            {
                Name = "ValidDuration",

                Passed = validDuration,

                Message = validDuration
                    ? "Quiz duration is valid."
                    : "Quiz duration must be greater than zero."
            });

            var validPassScore =
           quiz.PassScore >= 0 &&
           quiz.PassScore <= 100;

            checks.Add(new ReadinessCheckDto
            {
                Name = "ValidPassScore",

                Passed = validPassScore,

                Message = validPassScore
                    ? "Quiz pass score is valid."
                    : "Quiz pass score must be between 0 and 100."
            });

            var canPublish = checks.All(check => check.Passed);

            var result = new QuizReadinessDto
            {
                QuizId = quiz.Id,
                CanPublish = canPublish,
                Checks = checks
            };

            return RequestResponse<QuizReadinessDto>.Ok(
                result,
                "Quiz readiness checked successfully.");

        }
    }
}
