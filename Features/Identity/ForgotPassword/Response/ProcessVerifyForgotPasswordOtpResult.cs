namespace exam_system.Features.Identity.ForgotPassword.Response;

public enum PasswordResetOtpVerificationStatus
{
    Verified,
    InvalidCode,
    CodeExpired,
    AttemptsExceeded
}

public sealed record ProcessVerifyForgotPasswordOtpResult(
    PasswordResetOtpVerificationStatus Status,
    string? ResetToken = null);
