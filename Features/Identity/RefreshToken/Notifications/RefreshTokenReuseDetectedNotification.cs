using MediatR;

namespace exam_system.Features.Identity.RefreshTokens.Notifications;

public sealed record RefreshTokenReuseDetectedNotification(
    Guid RefreshTokenId,
    string UserId) : INotification;
