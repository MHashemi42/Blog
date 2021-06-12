using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Services
{
    public class MailKitEmailService : IEmailService
    {
        private readonly MailKitSettings _mailKitSettings;
        public MailKitEmailService(IOptions<MailKitSettings> mailKitSettings)
        {
            _mailKitSettings = mailKitSettings.Value;
        }
        public void Send(string to, string subject, string html)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_mailKitSettings.EmailFrom));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            smtp.Connect(_mailKitSettings.Host, _mailKitSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailKitSettings.Username, _mailKitSettings.Password);
            smtp.Send(message);
            smtp.Disconnect(true);
        }
    }
}
