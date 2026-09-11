using exam_system.Domain.Common;

namespace exam_system.Domain.Entities.Identity;

public class EmailVerificationOtp : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public string Email { get; set; } = string.Empty;
    public string OtpHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int AttemptCount { get; set; } = 0;
    public bool IsUsed { get; set; } = false;
}
