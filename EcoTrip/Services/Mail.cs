using EcoTrip.Models.DtoS;
using EcoTrip.Services.IServices;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;

namespace EcoTrip.Services
{
    public class Mail : IMail
    {
        private readonly IConfiguration _configuration;
        public Mail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMail(SendMailDto sendMailDto)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSettings:EmailUserName").Value));
            email.To.Add(MailboxAddress.Parse(sendMailDto.To));
            email.Subject = sendMailDto.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = sendMailDto.Body };

            using var smtp = new SmtpClient();

            smtp.Connect(_configuration.GetSection("EmailSettings:EmailHost").Value, 587, MailKit.Security.SecureSocketOptions.StartTls);

            smtp.Authenticate(_configuration.GetSection("EmailSettings:EmailUserName").Value, _configuration.GetSection("EmailSettings:EmailPassword").Value);

            smtp.Send(email);

            smtp.Disconnect(true);
        }
    }
}
