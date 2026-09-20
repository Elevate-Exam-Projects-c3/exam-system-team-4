using exam_system.Features.Identity.ForgotPassword.Commands;
using FluentValidation;

namespace exam_system.Features.Identity.ForgotPassword.Validators
{
    public sealed class VerifyForgotPasswordOtpCommandValidator
      : AbstractValidator<VerifyForgotPasswordOtpCommand>
    {
        public VerifyForgotPasswordOtpCommandValidator()
        {
            RuleFor(command => command.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Email is required")
                .MaximumLength(256)
                .WithMessage("Email cannot exceed 256 characters")
                .EmailAddress()
                .WithMessage("Email format is invalid");

            RuleFor(command => command.Otp)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("OTP is required")
                .Length(6)
                .WithMessage("OTP must be 6 digits")
                .Matches("^[0-9]{6}$")
                .WithMessage("OTP must contain digits only");
        }
    }
}
