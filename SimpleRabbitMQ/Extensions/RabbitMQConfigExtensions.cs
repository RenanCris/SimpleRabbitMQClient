using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Extensions
{
    internal static class RabbitMQConfigExtensions
    {
        public static RabbitMQConfig? GetConfigValue(this RabbitMQConfiguration? rabbitMQConfigValue, string connectionName)
        {
            return rabbitMQConfigValue?.RabbitMQConfig?.FirstOrDefault(x => x.Name == connectionName);
        }

        public static RabbitMQConfig? Valid(this RabbitMQConfig? rabbitMQConfigValue)
        {
            if (rabbitMQConfigValue is null)
            {
                throw new ConsumerAsyncException("RabbitMQConfig could not found by Connection Name or invalidad Name.", nameof(RabbitMQConfig));
            }

            return rabbitMQConfigValue;
        }

        public static RabbitMqExchangeOptions? Valid(this RabbitMqExchangeOptions? rabbitMQConfigValue)
        {
            if (rabbitMQConfigValue is null)
            {
                throw new ConsumerAsyncException("RabbitMqExchangeOptions could not found by Exchange Name.", nameof(RabbitMqExchangeOptions));
            }

            return rabbitMQConfigValue;
        }

        public static RabbitMqQueueOptions? Valid(this RabbitMqQueueOptions? rabbitMQConfigValue)
        {
            if (rabbitMQConfigValue is null)
            {
                throw new ConsumerAsyncException("RabbitMqQueueOptions could not found by Queue Name.", nameof(RabbitMqQueueOptions));
            }

            return rabbitMQConfigValue;
        }

        public static RabbitMqExchangeOptions? GetRabbitMqExchangeConfig(this RabbitMQConfig? rabbitMQConfig, string exchangeName)
        {
            return rabbitMQConfig?.Exchanges
                                       ?.FirstOrDefault(X => X.Name == exchangeName);
        }

        public static RabbitMqQueueOptions? GetQueueConfig(this RabbitMqExchangeOptions? rabbitMqExchangeOptions, string queueName)
        {
            return rabbitMqExchangeOptions
                                    ?.Queues.FirstOrDefault(x => x.Name == queueName);
        }
    }
}
