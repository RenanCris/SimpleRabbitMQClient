using SimpleRabbitMQ.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleRabbitMQ.Factories
{
    internal static class OutboxMessageFactory
    {
        public static OutboxMessage CreateByObject<T>(string connectionName, string exchangeName, string routingKey, T message)
        {
            if (string.IsNullOrEmpty(connectionName) || string.IsNullOrEmpty(exchangeName) ) throw new ArgumentNullException("[OutboxMessageFactory] Connection or Exchange was not declared.");

            var outObx = new OutboxMessage
            {
                ConnectionName = connectionName,
                ExchangeName = exchangeName,
                MessageData = JsonSerializer.Serialize(message),
                RoutingKey = routingKey

            };

            outObx.InitialValue();

            return outObx;
        }

        public static OutboxMessage Create(string connectionName, string exchangeName, string routingKey, string message)
        {
            if (string.IsNullOrEmpty(connectionName) || string.IsNullOrEmpty(exchangeName)) throw new ArgumentNullException("[OutboxMessageFactory] Connection or Exchange was not declared.");

            var outObx =  new OutboxMessage
            {
                ConnectionName = connectionName,
                ExchangeName = exchangeName,
                MessageData = message,
                RoutingKey = routingKey
            };

            outObx.InitialValue();

            return outObx;
        }
    }
}
