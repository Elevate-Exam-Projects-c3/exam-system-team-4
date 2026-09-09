using exam_system.Features.Quizzes.AdminCreateQuiz.ViewModels;
using exam_system.Features.Quizzes.AdminUpdateQuiz.ViewModels;
using FluentValidation;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Validators
{
    public class UpdateQuizValidator : AbstractValidator<UpdateQuizViewModel>
    {
        public UpdateQuizValidator()
        {
            RuleFor(quiz => quiz.Title)
                .NotEmpty()
                .Length(min: 3, max: 200).WithMessage("Title Length Must Be Between 3 and 200 characters");

            RuleFor(quiz => quiz.Instructions)
                .MaximumLength(2000).WithMessage("Instructions Length Mustn't Be Greater than 2000 Character")
                .When(quiz => !string.IsNullOrWhiteSpace(quiz.Instructions));

            RuleFor(quiz => quiz.DurationMinutes)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(quiz => quiz.PassScore)
                .InclusiveBetween(1, 100);


            RuleFor(quiz => quiz.MaxAttempts)
                .GreaterThan(0)
                .When(x => x.MaxAttempts.HasValue);

            RuleFor(quiz => quiz.StartDate)
           .NotEmpty();

            RuleFor(quiz => quiz.EndDate)
           .NotEmpty()
           .GreaterThan(quiz => quiz.StartDate)
           .WithMessage("End date must be after start date.");
        }
    }
}
