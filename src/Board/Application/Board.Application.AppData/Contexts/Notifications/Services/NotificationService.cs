using MassTransit;
using Notifier.Contracts.Contexts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public NotificationService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task SendMessage(string receiver, string subject, string body)
        {
            var message = new NotificationDetails { Receiver = receiver, Subject = subject, Body = body };
            return _publishEndpoint.Publish(message);
        }
    }
}
