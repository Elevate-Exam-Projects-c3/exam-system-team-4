using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminPublishQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminPublishQuiz.Handlers
{
    public class UnpublishQuizCommandHandler(IUnitOfWork unitOfWork,IGenericRepository<Quiz> repository) : IRequestHandler<UnpublishQuizCommand, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(UnpublishQuizCommand request, CancellationToken cancellationToken)
        {
            var quiz = await  repository
          .GetAll()
          .FirstOrDefaultAsync(
              x => x.Id == request.QuizId,
              cancellationToken);

            if (quiz == null)
            {
                throw new KeyNotFoundException(
                    $"Quiz with id '{request.QuizId}' was not found.");
            }

            quiz.Status = QuizStatus.Draft;
            quiz.PublishedAt = null;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<bool>.Ok(true);
        }
    }
}
