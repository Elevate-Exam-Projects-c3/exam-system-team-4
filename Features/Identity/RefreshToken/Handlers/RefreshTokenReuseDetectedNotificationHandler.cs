using exam_system.Features.Identity.RefreshTokens.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace exam_system.Features.Identity.RefreshTokens.Handlers;

public sealed class RefreshTokenReuseDetectedNotificationHandler
    : INotificationHandler<RefreshTokenReuseDetectedNotification>
{
    private readonly ILogger<RefreshTokenReuseDetectedNotificationHandler> _logger;

    public RefreshTokenReuseDetectedNotificationHandler(
        ILogger<RefreshTokenReuseDetectedNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        RefreshTokenReuseDetectedNotification notification,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Refresh token reuse detected for user {UserId} and refresh token record {RefreshTokenId}. Possible session compromise.",
            notification.UserId,
            notification.RefreshTokenId);

        return Task.CompletedTask;
    }
}
