using exam_system.Features.Identity.ForgotPassword.Commands;
using FluentValidation;

namespace exam_system.Features.Identity.ForgotPassword.Validators;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(command => command.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters")
            .EmailAddress()
            .WithMessage("Email format is invalid");
    }
}
