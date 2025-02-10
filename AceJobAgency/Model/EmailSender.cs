using AceJobAgency.Model;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly SmtpSettings _smtpSettings;

    public EmailSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_smtpSettings.Username), // Set the from address
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(new MailAddress(email)); // Add the recipient's email address

        using (var smtp = new SmtpClient())
        {
            smtp.Host = _smtpSettings.Host ?? throw new ArgumentNullException(nameof(_smtpSettings.Host));
            smtp.Port = _smtpSettings.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
            smtp.Send(message);
        }

        return Task.CompletedTask;
    }
}