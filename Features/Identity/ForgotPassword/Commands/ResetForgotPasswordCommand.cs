namespace exam_system.Features.Identity.ForgotPassword.Commands
{
    public sealed record ResetForgotPasswordCommand(
        string Email,
        string ResetToken,
        string NewPassword,
        string ConfirmPassword)
        : exam_system.Features.Shared.Cqrs.ITransactionalCommand<exam_system.Features.Shared.RequestResponse>;
}
