using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers
{
    public class ValidateQuestionForAttemptQueryHandler(IGenericRepository<QuizAttempt> repository) : IRequestHandler<ValidateQuestionForAttemptQuery, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(ValidateQuestionForAttemptQuery request, CancellationToken cancellationToken)
        {
            var attempt = await repository
              .GetAll()
              .Include(x => x.Quiz)
              .ThenInclude(x => x.Questions)
              .FirstOrDefaultAsync(
                  x => x.Id == request.AttemptId,
                  cancellationToken);

            if (attempt is null)
            {
                return RequestResponse<bool>.Fail(
                    "Attempt not found.",
                    404);
            }

            var questionExists = attempt.Quiz.Questions
                .Any(x => x.Id == request.QuestionId);

            if (!questionExists)
            {
                return RequestResponse<bool>.Fail(
                    "The question does not belong to this attempt's quiz.",
                    400);
            }

            return RequestResponse<bool>.Ok(
                true

                );
        }
    }
}
