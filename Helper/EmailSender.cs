using exam_system.Helper;
using MailKit.Security;
using MimeKit;

namespace exam_system.Infrastructure.Email;

public sealed class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string email,
        string subject,
        string htmlMessage,
        CancellationToken cancellationToken)
    {
        var host = GetRequiredSetting("EmailSettings:Host");

        var senderName = GetRequiredSetting(
            "EmailSettings:SenderName");

        var senderEmail = GetRequiredSetting(
            "EmailSettings:SenderEmail");

        var appPassword = GetRequiredSetting(
                "EmailSettings:AppPassword")
            .Replace(" ", string.Empty);

        var port = _configuration.GetValue<int>(
            "EmailSettings:Port");

        if (port <= 0)
        {
            throw new InvalidOperationException(
                "Email SMTP port is missing or invalid.");
        }

        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(senderName, senderEmail));

        message.To.Add(
            MailboxAddress.Parse(email.Trim()));

        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlMessage
        }.ToMessageBody();

        using var smtpClient =
            new MailKit.Net.Smtp.SmtpClient();

        try
        {
            await smtpClient.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls,
                cancellationToken);

            await smtpClient.AuthenticateAsync(
                senderEmail,
                appPassword,
                cancellationToken);

            await smtpClient.SendAsync(
                message,
                cancellationToken);
        }
        finally
        {
            if (smtpClient.IsConnected)
            {
                await smtpClient.DisconnectAsync(
                    true,
                    CancellationToken.None);
            }
        }
    }

    private string GetRequiredSetting(string key)
    {
        var value = _configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Email setting '{key}' is missing.");
        }

        return value.Trim();
    }
}