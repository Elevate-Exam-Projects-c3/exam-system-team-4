namespace exam_system.Helper
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email,string subject,string htmlMessage,CancellationToken cancellationToken);
    }
}
