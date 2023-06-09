﻿using Notifier.Contracts.Contexts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifier.Application.AppData.Contexts.Messages.Services
{
    public interface INotificationService
    {
        Task SendAsync(NotificationDetails message, CancellationToken cancellation);
    }
}
