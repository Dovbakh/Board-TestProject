using Board.Application.AppData.Contexts.AdvertFavorites.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        public Task SendMessage(NotificationDetails message)
        {
            _logger.LogInformation("{0}:{1} -> Отправка сообщения. {2}: {3}",
                nameof(NotificationService), nameof(SendMessage), nameof(NotificationDetails), JsonConvert.SerializeObject(message));

            return _publishEndpoint.Publish(message);
        }
    }
}
