using exam_system.Common.Enums;
using exam_system.Features.Attempts.StartAttempt.Commands;
using FluentValidation;
namespace exam_system.Features.Attempts.StartAttempt.Validators
{
    public class CreateQuizAttemptCommandValidator
        : AbstractValidator<CreateQuizAttemptCommand>
    {
        public CreateQuizAttemptCommandValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty();

            RuleFor(x => x.QuizId)
                .NotEmpty();

            RuleFor(x => x.StartTime)
                .NotEmpty();

            RuleFor(x => x.Deadline)
                .GreaterThan(x => x.StartTime);

            RuleFor(x => x.attemptStatus)
                .Equal(AttemptStatus.InProgress);
        }
    }
}