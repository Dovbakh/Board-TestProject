using MassTransit;
using Microsoft.Extensions.Logging;
using Notifier.Contracts.Contexts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Notifications.Services
{
    /// <inheritdoc />
    public class NotificationService : INotificationService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IPublishEndpoint publishEndpoint, ILogger<NotificationService> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task SendMessage(string receiver, string subject, string body)
        {
            _logger.LogInformation("{0} -> Отправка сообщения. {1}: {2}, {3}: {4}, {5}: {6}",
                nameof(SendMessage), nameof(receiver), receiver, nameof(subject), subject, nameof(body), body);

            var message = new NotificationDetails { Receiver = receiver, Subject = subject, Body = body };
            return _publishEndpoint.Publish(message);
        }
    }
}
