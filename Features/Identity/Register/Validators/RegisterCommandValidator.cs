using exam_system.Features.Identity.Register.Commands;
using FluentValidation;

namespace exam_system.Features.Identity.Register.Validators
{
    public sealed class RegisterCommandValidator: AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(command => command.FullName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Full name is required")
                .Must(fullName => fullName.Trim().Length >= 2)
                .WithMessage("Full name must be at least 2 characters")
                .Must(fullName => fullName.Trim().Length <= 100)
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
