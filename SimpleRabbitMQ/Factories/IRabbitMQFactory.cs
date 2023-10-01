using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SimpleRabbitMQ.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Factories
{
    public interface IRabbitMQFactory
    {
        IConnection? CreateRabbitMqConnection(RabbitMQConfig? options);
        AsyncEventingBasicConsumer CreateConsumer(IModel channel);
    }
}
