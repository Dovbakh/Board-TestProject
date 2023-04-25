using Microsoft.Extensions.Configuration;
using Notifier.Contracts.Contexts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Notifier.Application.AppData.Contexts.Messages.Services
{
    public class MessageService : IMessageService
    {
        private readonly IConfiguration _configuration;

        public MessageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(MessageDetails message, CancellationToken cancellation)
        {
            var senderEmail = _configuration.GetSection("EmailService").GetSection("EmailUsername").Value;
            var senderPassword = _configuration.GetSection("EmailService").GetSection("EmailPassword").Value;
            var smtpHost = _configuration.GetSection("EmailService").GetSection("SmtpHost").Value;
            var smtpPort = int.Parse(_configuration.GetSection("EmailService").GetSection("SmtpPort").Value);
            
            var email = new MailMessage(senderEmail, message.Receiver, message.Subject, message.Body);
            var client = new SmtpClient(smtpHost, smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);

            try
            {
                await client.SendMailAsync(email, cancellation);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
