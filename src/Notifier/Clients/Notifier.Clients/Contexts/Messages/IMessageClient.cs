using Notifier.Contracts.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifier.Clients.Contexts.Messages
{
    public interface IMessageClient
    {
        Task Send(MessageDetailsClientRequest clientRequest, CancellationToken cancellation);
    }
}
