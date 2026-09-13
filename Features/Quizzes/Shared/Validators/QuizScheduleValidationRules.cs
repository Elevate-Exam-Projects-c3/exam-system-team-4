using FluentValidation;

namespace exam_system.Features.Quizzes.Shared.Validators
{
    public static class QuizScheduleValidationRules
    {
        public static void ApplyQuizScheduleRules<T>(this AbstractValidator<T> validator)
                        where T : IQuizScheduleRequest
        {
            validator.RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Start date must be in the future.");

            validator.RuleFor(x => x.EndDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("End date must be in the future.")
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");

            validator.RuleFor(x => x)
                .Must(x => (x.EndDate - x.StartDate).TotalMinutes >= x.DurationMinutes)
                .WithMessage("Duration cannot exceed the time between start date and end date.")
                .WithName("DurationMinutes");
        }
    }
}
