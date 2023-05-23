using Notifier.Contracts.Contexts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Notifications.Services
{
    /// <summary>
    /// Сервис для работы с уведомлениями.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Отправление уведомления.
        /// </summary>
        /// <param name="message">Модель уведомления.</param>
        Task SendMessage(NotificationDetails message);
    }
}
