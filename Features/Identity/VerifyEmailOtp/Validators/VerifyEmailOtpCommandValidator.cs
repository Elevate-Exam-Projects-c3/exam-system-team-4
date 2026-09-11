using exam_system.Features.Identity.VerifyEmailOtp.Commands;
using FluentValidation;

namespace exam_system.Features.Identity.VerifyEmailOtp.Validators;

public sealed class VerifyEmailOtpCommandValidator
    : AbstractValidator<VerifyEmailOtpCommand>
{
    public VerifyEmailOtpCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");

        RuleFor(command => command.Otp)
            .NotEmpty()
            .WithMessage("OTP is required.")
            .Matches(@"^\d{6}$")
            .WithMessage("OTP must contain exactly 6 digits.");
    }
}
