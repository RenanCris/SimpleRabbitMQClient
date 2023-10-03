using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Extensions;
using SimpleRabbitMQ.Factories;
using SimpleRabbitMQ.Services.Interfaces;

namespace SimpleRabbitMQ.Services
{
    public abstract class ConsumerAsyncJsonObjectBase<TMessage> : ConsumerAsyncBase 
    {
        protected ConsumerAsyncJsonObjectBase(ILoggingService loggingService, IRabbitMQFactory rabbitMQFactory, IOptions<RabbitMQConfiguration> rabbitMQConfig, string connectionName, string exchangeName, string queueName, ushort prefetchCount = 0) : base(loggingService, rabbitMQFactory, rabbitMQConfig, connectionName, exchangeName, queueName, prefetchCount)
        {
        }

        protected TMessage? MessageObject { get; private set; }

        public override async Task HandleMessagesAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken)
        {
            try
            {
                MessageObject = message.GetPayload<TMessage>();
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Error while deserializing the object : {nameof(TMessage)}, message : {message.GetMessage()}");
                throw;
            }

            await HandleMessageClientAsync(message, cancellationToken);
        }
    }
}
