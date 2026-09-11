namespace exam_system.Features.Identity.VerifyEmailOtp.Dtos.Response;

public sealed record VerifyEmailOtpResponse(string UserId,string Email,bool EmailConfirmed,string AccountStatus);
