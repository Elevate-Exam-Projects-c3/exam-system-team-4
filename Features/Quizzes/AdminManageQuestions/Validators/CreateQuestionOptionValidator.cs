using exam_system.Features.Quizzes.AdminManageQuestions.Dto;
using FluentValidation;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Validators
{
    public class CreateQuestionOptionValidator : AbstractValidator<QuestionOptionDto>
    {
        public CreateQuestionOptionValidator()
        {
            RuleFor(x => x.OptionText)
            .NotEmpty()
            .WithMessage("Option text is required.")
            .MaximumLength(500)
            .WithMessage("Option text cannot exceed 500 characters.");
        }
    }
}
