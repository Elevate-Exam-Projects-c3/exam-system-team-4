using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminManageQuestions.Commands;
using exam_system.Features.Quizzes.AdminManageQuestions.Dto;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Handlers
{
    public class AddQuestionCommandHandler(
        IUnitOfWork unitOfWork,
        IGenericRepository<Question> questionRepository)
        : IRequestHandler<CreateQuestionCommand, RequestResponse<bool>>
    {



        public async Task<RequestResponse<bool>> Handle(
            CreateQuestionCommand request,
            CancellationToken cancellationToken)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                QuizId = request.QuizId,
                Text = request.QuestionText,
                Explanation = request.Explanation,
                OrderIndex = request.OrderIndex,
                CreatedAt = DateTime.UtcNow,
            };

            foreach (var optionDto in request.Options)
            {
                question.Options.Add(new QuestionOption
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    OptionText = optionDto.OptionText,
                    IsCorrect = optionDto.IsCorrect
                });
            }

            
            questionRepository.Add(question);

            
            await unitOfWork.SaveChangesAsync(cancellationToken);


            return RequestResponse<bool>.Ok(true);
           
        }
    }
}