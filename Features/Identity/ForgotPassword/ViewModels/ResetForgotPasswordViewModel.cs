namespace exam_system.Features.Identity.ForgotPassword.ViewModels
{
    public sealed record ResetForgotPasswordViewModel(
        string Email,
        string ResetToken,
        string NewPassword,
        string ConfirmPassword);
}
