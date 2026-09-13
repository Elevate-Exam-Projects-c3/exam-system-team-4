using exam_system.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
   
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Pending;

    // Navigation for 1-to-1 relationship with Student
    public Student? Student { get; set; }

    // Navigation for security tokens
    public ICollection<EmailVerificationOtp> EmailVerificationOtps { get; set; } = new List<EmailVerificationOtp>();
    public ICollection<PasswordResetOtp> PasswordResetOtps { get; set; } = new List<PasswordResetOtp>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
