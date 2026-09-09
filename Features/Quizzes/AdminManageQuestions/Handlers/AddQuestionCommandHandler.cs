using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminManageQuestions.Commands;
using exam_system.Features.Quizzes.AdminManageQuestions.Dto;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Handlers
{
    public class AddQuestionCommandHandler(
        IUnitOfWork unitOfWork,
        IGenericRepository<Question> questionRepository)
        : IRequestHandler<CreateQuestionCommand, QuestionDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Question> _questionRepository = questionRepository;

        public async Task<QuestionDto> Handle(
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

            
            _questionRepository.Add(question);

            
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return new QuestionDto
            {
                Id = question.Id,
                QuizId = question.QuizId,
                Text = question.Text,
                Explanation = question.Explanation,
                OrderIndex = question.OrderIndex,

                Options = question.Options
                    .Select(x => new QuestionOptionDto
                    {
                        Id = x.Id,
                        OptionText = x.OptionText,
                        IsCorrect = x.IsCorrect
                    })
                    .ToList()
            };

        }
    }
}