using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Services.Interfaces
{
    public interface IConsumerServiceAsync : IHostedService, IDisposable
    {
        Task HandleMessagesAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken);
    }
}
