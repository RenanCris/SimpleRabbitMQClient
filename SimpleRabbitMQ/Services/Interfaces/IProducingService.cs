using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Services.Interfaces
{
    public interface IProducingService : IProducingBaseService
    {
        void UseDataConnection(IConnection connection, IModel channel, string connectionName);
        IProducingService SetConnectionName(string connectionName);
    }
}
