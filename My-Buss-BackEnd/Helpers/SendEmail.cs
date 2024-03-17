using My_Buss_BackEnd.Interfaces;
using My_Buss_BackEnd.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace My_Buss_BackEnd.Helpers
{
    public class SendEmail(IOptions<GmailSettings> mailSettings) : IMessage
    {
        public GmailSettings MailSettings { get; } = mailSettings.Value;
        public void EmailConfig(string to, string subject, string body)
        {
            try
            {
                var fromEmail = MailSettings.Username!;
                var password = MailSettings.Password;
                MailMessage message = new()
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject
                };
                message.To.Add(new MailAddress(to));
                message.Body = body;
                message.IsBodyHtml = true;

                SmtpClient smtp = new("smtp.gmail.com")
                {
                    Port = MailSettings.Port,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true
                };

                smtp.Send(message);
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
