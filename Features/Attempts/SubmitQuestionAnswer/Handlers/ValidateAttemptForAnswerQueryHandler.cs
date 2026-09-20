using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers
{
    public class ValidateAttemptForAnswerQueryHandler(IGenericRepository<QuizAttempt> repository) : IRequestHandler<ValidateAttemptForAnswerQuery, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(ValidateAttemptForAnswerQuery request, CancellationToken cancellationToken)
        {
            var attempt = await repository
      .GetAll()
      .FirstOrDefaultAsync(
          x =>
              x.Id == request.AttemptId &&
              x.StudentId == request.StudentId,
          cancellationToken);

            if (attempt == null)
            {
                return RequestResponse<bool>.Fail("Attempt not found", 404);
            }

            var now = DateTime.UtcNow;

            if (now < attempt.StartTime || now > attempt.Deadline)
            {
                return RequestResponse<bool>.Fail(
                    "Attempt is outside the valid time window",
                    410);
            }

            return RequestResponse<bool>.Ok(true, "Attempt is valid for answering");

        }
            
    }
}
