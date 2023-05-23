using Board.Contracts.Contexts.Users;
using MassTransit;
using Notifier.Application.AppData.Contexts.Messages.Services;
using Notifier.Contracts.Contexts.Messages;

namespace Notifier.Host.Consumer
{
    public class MassTransitConsumer : IConsumer<NotificationDetails>
    {
        private readonly INotificationService _notificationService;

        public MassTransitConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<NotificationDetails> context)
        {
            var message = context.Message;
            await Console.Out.WriteLineAsync($"Message from Producer : {message.Body}, {message.Receiver}");
            await _notificationService.SendAsync(context.Message, context.CancellationToken);        
        }
    }
}
