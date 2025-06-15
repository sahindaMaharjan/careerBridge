using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace careerBridge.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender (IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string Subject, string msg)
        {
            var email = _config["EmailSettings:Email"];
            var password = _config["EmailSettings:AppPassword"];

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential(email, password);
                client.EnableSsl = true;

                var message = new MailMessage(email, toEmail, Subject, msg);
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);
            }

        }
    }
}
