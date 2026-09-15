using exam_system.Helper;
using MediatR;

namespace exam_system.Features.Identity.Register.Notification
{
    public sealed record SendVerificationOtpNotification(
    string Email,
    string FullName,
    string Otp) : INotification;

    public class SendVerificationOtpNotificationHandler : INotificationHandler<SendVerificationOtpNotification>
    {
        private readonly IEmailSender _emailSender;

        public SendVerificationOtpNotificationHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public async Task Handle(SendVerificationOtpNotification notification, CancellationToken cancellationToken)
        {
            var subject = "Verify your email";

            var htmlMessage = $"""
            <h2>Hello {notification.FullName}</h2>
            <p>Your verification code is:</p>
            <h1>{notification.Otp}</h1>
            <p>This code expires in 10 minutes.</p>
            """;

            await _emailSender.SendEmailAsync(notification.Email,subject, htmlMessage, cancellationToken);
        }
    }
}

