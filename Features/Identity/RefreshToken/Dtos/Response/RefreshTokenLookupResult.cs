namespace exam_system.Features.Identity.RefreshTokens.Dtos.Response;

public sealed record RefreshTokenLookupResult(
    Guid Id,
    string UserId,
    DateTime ExpiresAt,
    bool IsUsed,
    bool IsRevoked);
