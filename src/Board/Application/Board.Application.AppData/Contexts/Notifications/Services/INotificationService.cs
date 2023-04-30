using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Notifications.Services
{
    public interface INotificationService
    {
        Task SendMessage(string receiver, string subject, string body);
    }
}
