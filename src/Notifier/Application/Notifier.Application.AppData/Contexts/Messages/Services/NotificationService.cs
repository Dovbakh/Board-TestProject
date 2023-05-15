using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Notifier.Contracts.Contexts.Messages;
using Notifier.Contracts.Options;
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
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly SmtpOptions _smtpOptions;

        public NotificationService(ILogger<NotificationService> logger, IOptions<SmtpOptions> smtpOptionsAccessor)
        {
            _logger = logger;
            _smtpOptions = smtpOptionsAccessor.Value;
        }

        /// <inheritdoc />
        public async Task SendAsync(NotificationDetails message, CancellationToken cancellation)
        {
            _logger.LogInformation("{0}:{1} -> Отправление сообщения из указанной модели: {2}",
                nameof(NotificationService), nameof(SendAsync), JsonConvert.SerializeObject(message));
            
            var email = new MailMessage(_smtpOptions.Email, message.Receiver, message.Subject, message.Body);
            email.IsBodyHtml = true;

            var client = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_smtpOptions.Email, _smtpOptions.Password);

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
