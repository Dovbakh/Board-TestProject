using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    /// <inheritdoc />
    public class MessageService : IMessageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IConfiguration configuration, ILogger<MessageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SendAsync(NotificationDetails message, CancellationToken cancellation)
        {
            _logger.LogInformation("{0} -> Отправление сообщения из указанной модели: {1}",
                nameof(SendAsync), JsonConvert.SerializeObject(message));

            var senderEmail = _configuration.GetSection("EmailService").GetSection("EmailUsername").Value;
            var senderPassword = _configuration.GetSection("EmailService").GetSection("EmailPassword").Value;
            var smtpHost = _configuration.GetSection("EmailService").GetSection("SmtpHost").Value;
            var smtpPort = int.Parse(_configuration.GetSection("EmailService").GetSection("SmtpPort").Value);
            
            var email = new MailMessage(senderEmail, message.Receiver, message.Subject, message.Body);
            email.IsBodyHtml = true;
            var client = new SmtpClient(smtpHost, smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);

            try
            {
                await client.SendMailAsync(email, cancellation);
            }
            catch (Exception ex)
            {
                throw new Exception("При отправлении сообщения произошла ошибка.", ex);
            }
            
        }
    }
}
