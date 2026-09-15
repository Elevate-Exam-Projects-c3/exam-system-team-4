using exam_system.Common.Enums;

namespace exam_system.Features.Shared.UserLookup.Dtos.Response;

public sealed record UserLookupResult(
    string UserId,
    string? Email,
    AccountStatus AccountStatus,
    bool EmailConfirmed,
    DateTimeOffset? LockoutEnd,
    Guid? StudentId);
