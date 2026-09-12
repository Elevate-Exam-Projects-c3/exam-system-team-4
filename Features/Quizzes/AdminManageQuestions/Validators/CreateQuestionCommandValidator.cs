using exam_system.Features.Quizzes.AdminManageQuestions.Commands;
using exam_system.Features.Quizzes.AdminManageQuestions.Orchestrators;
using FluentValidation;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Validators
{
    public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionOrchestrator>
    {
        public CreateQuestionCommandValidator()
        {
            RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("QuizId is required.");

            RuleFor(x => x.QuestionText)
                .NotEmpty()
                .WithMessage("Question text is required.")
                .MaximumLength(1000)
                .WithMessage("Question text cannot exceed 1000 characters.");

            RuleFor(x => x.Options)
                .NotNull()
                .WithMessage("Options are required.")
                .Must(options => options.Count >= 2)
                .WithMessage("Question must have at least 2 options.");

            RuleFor(x => x.Options)
                .Must(options => options.Count(option => option.IsCorrect) == 1)
                .WithMessage("Question must have exactly one correct option.");

            RuleForEach(x => x.Options)
                .SetValidator(new CreateQuestionOptionValidator());
        }
    }
}
