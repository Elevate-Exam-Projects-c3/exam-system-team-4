using exam_system.Features.Identity.ForgotPassword.Commands;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace exam_system.Features.Identity.ForgotPassword.Validators;

public sealed class ResetForgotPasswordCommandValidator : AbstractValidator<ResetForgotPasswordCommand>
{
    public ResetForgotPasswordCommandValidator(IOptions<IdentityOptions> identityOptions)
    {
        var rules = identityOptions.Value.Password;
        RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
            .NotEmpty().MaximumLength(256).EmailAddress();
        RuleFor(x => x.ResetToken).Cascade(CascadeMode.Stop)
            .NotEmpty().Matches("\\A[0-9A-Fa-f]{64}\\z")
            .WithMessage("Invalid reset token.");
        RuleFor(x => x.NewPassword).Cascade(CascadeMode.Stop)
            .NotEmpty().MinimumLength(rules.RequiredLength)
            .Must(password => password.Distinct().Count() >= rules.RequiredUniqueChars)
            .WithMessage($"Password must contain at least {rules.RequiredUniqueChars} unique characters.");

        When(_ => rules.RequireDigit, () =>
            RuleFor(x => x.NewPassword)
                .Must(password => password?.Any(c => c >= '0' && c <= '9') == true)
                .WithMessage("Password must contain a digit."));
        When(_ => rules.RequireLowercase, () =>
            RuleFor(x => x.NewPassword)
                .Must(password => password?.Any(c => c >= 'a' && c <= 'z') == true)
                .WithMessage("Password must contain a lowercase letter."));
        When(_ => rules.RequireUppercase, () =>
            RuleFor(x => x.NewPassword)
                .Must(password => password?.Any(c => c >= 'A' && c <= 'Z') == true)
                .WithMessage("Password must contain an uppercase letter."));
        When(_ => rules.RequireNonAlphanumeric, () =>
            RuleFor(x => x.NewPassword)
                .Must(password => password?.Any(c =>
                    !(c >= '0' && c <= '9') && !(c >= 'a' && c <= 'z') &&
                    !(c >= 'A' && c <= 'Z')) == true)
                .WithMessage("Password must contain a non-alphanumeric character."));

        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}
