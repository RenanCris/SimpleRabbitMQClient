using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Factories;
using SimpleRabbitMQ.Services.Interfaces;
using SimpleRabbitMQ.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Registrations.Builders
{

    internal class InicializeConfiguration : IInicializeConfiguration
    {
        private readonly RabbitMQConfiguration _rabbitMQConfiguration1;
        private readonly IRabbitMQFactory _rabbitMQFactory;
        private readonly ILoggingService _logger;
        public InicializeConfiguration(ILoggingService logger, IOptions<RabbitMQConfiguration> rabbitMQConfiguration, IRabbitMQFactory rabbitMQFactory)
        {
            _rabbitMQConfiguration1 = rabbitMQConfiguration.Value;

            if (rabbitMQConfiguration == null) throw new ArgumentException("Unspecified RabbitMQConfiguration, please describe the settings in the 'appsettings' of your client application. ");

            _rabbitMQFactory = rabbitMQFactory;
            _logger = logger;
        }

        public void Builder() 
        {
            _logger.LogInformation("[InicializeConfiguration] Starting the setup of the RabbitMQ structure.");

            foreach (var config in _rabbitMQConfiguration1.RabbitMQConfig)
            {
                try
                {
                    var connection = _rabbitMQFactory.CreateRabbitMqConnection(config);

                    using var channel = connection?.CreateModel();

                    config.Exchanges.ToList().ForEach(exchange =>
                    {
                        bool isUseDeadLetter = DeclarerExchangeConfig(exchange, channel);

                        exchange.Queues.ToList().ForEach(q =>
                        {
                            DeclarerQueueConfig(exchange, q, channel, isUseDeadLetter);
                            DeclarerConfigDeadLetterBind(exchange, q, channel, isUseDeadLetter);
                        });
                    });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[InicializeConfiguration] Erro Configure Name: {config.Name}");
                }
            }
        }

        private static void DeclarerConfigDeadLetterBind(RabbitMqExchangeOptions exchange, RabbitMqQueueOptions q, RabbitMQ.Client.IModel channel, bool isUseDeadLetter)
        {
            if (isUseDeadLetter)
            {
                var deadLetterQueue = string.Concat(q.Name, "-dead-letter");
                var deadLetterRK = string.Concat(q.RoutingKey, "-dead-letter");

                channel.QueueDeclare(queue: deadLetterQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueBind(deadLetterQueue, exchange.DeadLetterExchange, deadLetterRK, null);
            }
        }

        private void DeclarerQueueConfig(RabbitMqExchangeOptions exchange, RabbitMqQueueOptions q, RabbitMQ.Client.IModel channel, bool isUseDeadLetter)
        {
            var argumentsQueue = new Dictionary<string, object>();


            if (q.Arguments.Any())
                q.Arguments.ToList().ForEach(x => argumentsQueue.Add(x.Key, x.Value));

            if (isUseDeadLetter)
            {
                argumentsQueue.Add("x-dead-letter-exchange", exchange.DeadLetterExchange);

                if (exchange.DeadLetterExchangeType == "direct") 
                {
                    var deadLetterRK = string.Concat(q.RoutingKey, "-dead-letter");
                    argumentsQueue.Add("x-dead-letter-routing-key", deadLetterRK);
                }
            }

            new RabbitMqQueueValidation(q).ThrowException("The Queue Name was not configured.");

            channel.QueueDeclare(queue: q.Name,
                 durable: q.Durable,
                 exclusive: q.Exclusive,
                 autoDelete: q.AutoDelete,
                 arguments: argumentsQueue);

            channel.QueueBind(q.Name, exchange.Name, q.RoutingKey, null);

            _logger.LogInformation($"[InicializeConfiguration] ExchangeDeclare Queue Name {q.Name}");
        }

        private bool DeclarerExchangeConfig(RabbitMqExchangeOptions exchange, IModel channel)
        {
            new RabbitMqExchangeValidation(exchange).ThrowException("The Exchange Name was not configured.");

            channel.ExchangeDeclare(exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Arguments);

            _logger.LogInformation($"[InicializeConfiguration] ExchangeDeclare : {exchange.Name}");

            var isUseDeadLetter = !string.IsNullOrEmpty(exchange.DeadLetterExchange);

            if (isUseDeadLetter)
            {
                channel.ExchangeDeclare(exchange.DeadLetterExchange, exchange.DeadLetterExchangeType, true, false, null);

                _logger.LogInformation($"[InicializeConfiguration] DeadLetterExchange : {exchange.DeadLetterExchange}");
            }

            _logger.LogInformation($"[InicializeConfiguration] ExchangeDeclare Queues: {exchange.Queues.Count}");
            return isUseDeadLetter;
        }
    }
}
