using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminManageQuestions.Commands;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Handlers
{
    public class DeleteQuestionCommandHandler(IGenericRepository<Question> repository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteQuestionCommand, string>
    {
        private readonly IGenericRepository<Question> _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var question = await _repository
          .GetAll()
          .Include(x => x.Options)
          .Include(x => x.Quiz)
          .FirstOrDefaultAsync(
              x => x.Id == request.QuestionId,
              cancellationToken);

            if (question == null)
                throw new KeyNotFoundException("Question not found.");


            if (question.Quiz.Status == Common.Enums.QuizStatus.Published)
                throw new InvalidOperationException(
                    "Cannot delete a question from a published quiz. Unpublish the quiz first.");


            question.IsDeleted = true;
            question.DeletedAt = DateTime.Now;

            foreach (var option in question.Options)
            {
                option.IsDeleted = true;
                option.DeletedAt = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return "Question deleted successfully.";
        }
    }
}
