using exam_system.Features.Identity.Register.Commands;
using FluentValidation;

namespace exam_system.Features.Identity.Register.Validators
{
    public sealed class RegisterCommandValidator: AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(command => command.FullName)
                .NotEmpty()
                .WithMessage("Full name is required")
                .MinimumLength(2)
                .WithMessage("Full name must be at least 2 characters")
                .MaximumLength(100)
                .WithMessage("Full name cannot exceed 100 characters");

            RuleFor(command => command.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email format is invalid");

            RuleFor(command => command.Password)
                .NotEmpty()
                .WithMessage("Password is required");
        }
    }
}
