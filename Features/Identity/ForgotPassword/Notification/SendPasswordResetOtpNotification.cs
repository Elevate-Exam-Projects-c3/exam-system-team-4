using exam_system.Helper;
using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Notification;

public sealed record SendPasswordResetOtpNotification( string Email,string Otp) : INotification;

public sealed class SendPasswordResetOtpNotificationHandler
    : INotificationHandler<SendPasswordResetOtpNotification>
{
    private readonly IEmailSender _emailSender;

    public SendPasswordResetOtpNotificationHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task Handle(SendPasswordResetOtpNotification notification, CancellationToken cancellationToken)
    {
        var htmlMessage = $"""
            <p>Your password reset code is:</p>
            <h1>{notification.Otp}</h1>
            <p>This code expires in 10 minutes.</p>
            """;

        return _emailSender.SendEmailAsync(notification.Email, "Reset your password", htmlMessage, cancellationToken);
    }
}
