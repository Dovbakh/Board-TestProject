using AutoMapper;
using Microsoft.Extensions.Configuration;
using Notifier.Contracts.Clients;
using Notifier.Contracts.Contexts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Notifier.Clients.Contexts.Messages
{
    
    public class MessageClient : IMessageClient
    {
        private readonly HttpClient _httpClient;

        public MessageClient(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
        }       

        public async Task Send(MessageDetailsClientRequest sendClientRequest, CancellationToken cancellation)
        {
            var sendRequest = new NotificationDetails { Receiver = sendClientRequest.Receiver, Subject = sendClientRequest.Subject, Body = sendClientRequest.Body };

            var uri = $"v1/message";
            using var response = await _httpClient.PostAsJsonAsync(uri, sendClientRequest, cancellation);
            response.EnsureSuccessStatusCode();
        }
    }
}
