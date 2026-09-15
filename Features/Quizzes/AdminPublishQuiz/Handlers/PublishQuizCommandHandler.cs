using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminPublishQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminPublishQuiz.Handlers
{
    public class PublishQuizCommandHandler(IUnitOfWork unitOfWork,IGenericRepository<Quiz> repository) : IRequestHandler<PublishQuizCommand, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(PublishQuizCommand request, CancellationToken cancellationToken)
        {
            var quiz = await repository
          .GetAll()
          .FirstOrDefaultAsync(
              x => x.Id == request.QuizId,
              cancellationToken);

            if (quiz == null)
            {
                throw new KeyNotFoundException(
                    $"Quiz with id '{request.QuizId}' was not found.");
            }

            quiz.Status = QuizStatus.Published;
            quiz.PublishedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return  RequestResponse<bool>.Ok(true);
        }
    }
}
