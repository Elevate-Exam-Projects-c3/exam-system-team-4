using exam_system.Domain.Common;

namespace exam_system.Domain.Entities.Identity;

public class RefreshToken :BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    public bool IsRevoked { get; set; } = false;

    public string? ReplacedByTokenHash { get; set; }
}
