using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using PropMan.Models;
using MailKit.Security; // Ensure this is included

namespace Propman.Services
{

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            // *** MODIFIED CONNECTION LINE FOR PORT 465/SSLONCONNECT ***
            await smtp.ConnectAsync(_emailSettings.host, _emailSettings.Port, SecureSocketOptions.SslOnConnect); 
            await smtp.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
