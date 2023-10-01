using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Extensions;
using SimpleRabbitMQ.Factories;
using SimpleRabbitMQ.Services;
using SimpleRabbitMQ.Services.Interfaces;

namespace SimpleRabbitMQ.Validation.Infrastructure
{
    public class ConsumerTestAsync : ConsumerAsyncBase
    {
        public ConsumerTestAsync(ILoggingService loggingService, IRabbitMQFactory rabbitMQFactory, IOptions<RabbitMQConfiguration> rabbitMQConfig, string connectionName, string exchangeName, string queueName, ushort prefetchCount = 0) : base(loggingService, rabbitMQFactory, rabbitMQConfig, connectionName, exchangeName, queueName, prefetchCount)
        {
        }

        protected override Task HandleMessageAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken)
        {
            _loggingService.LogInformation(message.GetMessage());

            return Task.CompletedTask;
        }
    }
}
