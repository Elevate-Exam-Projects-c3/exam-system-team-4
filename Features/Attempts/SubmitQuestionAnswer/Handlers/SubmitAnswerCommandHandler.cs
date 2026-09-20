using exam_system.Domain.Entities.Attempts;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers
{
    public class SubmitAnswerCommandHandler(IGenericRepository<StudentQuestionAnswer> repository,IGenericRepository<QuestionOption> questionOptionRepository,
        IUnitOfWork unitOfWork  ) : IRequestHandler<SubmitAnswerCommand, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(SubmitAnswerCommand request, CancellationToken cancellationToken)
        {
            var answer = await repository
     .GetAll()
     .FirstOrDefaultAsync(
         x => x.AttemptId == request.AttemptId &&
              x.QuestionId == request.QuestionId,
         cancellationToken);

            bool? isCorrect = null;

            if (request.SelectedOptionId.HasValue)
            {
                isCorrect = await questionOptionRepository
                    .GetAll()
                    .Where(x => x.Id == request.SelectedOptionId.Value)
                    .Select(x => (bool?)x.IsCorrect)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (answer is null)
            {
                answer = new StudentQuestionAnswer
                {
                    AttemptId = request.AttemptId,
                    QuestionId = request.QuestionId,
                    SelectedOptionId = request.SelectedOptionId,
                    IsCorrect = isCorrect,
                    AnsweredAt = DateTime.UtcNow
                };

                repository.Add(answer);
            }
            else
            {
                answer.SelectedOptionId = request.SelectedOptionId;
                answer.IsCorrect = isCorrect;
                answer.AnsweredAt = DateTime.UtcNow;

                repository.Update(answer);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<bool>.Ok(
                true
                
                );
        }
    }
}
