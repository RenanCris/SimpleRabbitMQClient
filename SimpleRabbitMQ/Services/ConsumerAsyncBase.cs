using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Exceptions;
using SimpleRabbitMQ.Extensions;
using SimpleRabbitMQ.Factories;
using SimpleRabbitMQ.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Services
{
    public abstract class ConsumerAsyncBase : IConsumerServiceAsync
    {
        protected readonly ILoggingService _loggingService;
        private readonly string ConnectionName;
        private readonly string QueueName;
        private readonly string ExchangeName;

        private readonly IRabbitMQFactory _rabbitMQFactory;
        private readonly RabbitMQConfiguration _rabbitMQConfig;
        private string _consumerTags;
        private bool _disposed = false;

        public IConnection? Connection { get; private set; }
        public IModel? Channel { get; private set; }

        private readonly string ConsumerName;
        private readonly ushort PrefetchCount;

        protected ConsumerAsyncBase(ILoggingService loggingService,
            IRabbitMQFactory rabbitMQFactory,
            IOptions<RabbitMQConfiguration> rabbitMQConfig,
            string connectionName,
            string exchangeName,
            string queueName,
            ushort prefetchCount = 0)
        {
            _loggingService = loggingService;
            ConnectionName = connectionName;
            QueueName = queueName;
            ExchangeName = exchangeName;
            PrefetchCount = prefetchCount;

            _rabbitMQFactory = rabbitMQFactory;
            _rabbitMQConfig = rabbitMQConfig.Value;


            ValidateProperties();

            ConsumerName = string.Concat(this.GetType().Name, "_", Guid.NewGuid());
        }

        protected abstract Task HandleMessageAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken);

        public async Task HandleMessagesAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken)
        {
            await HandleMessageAsync(message, cancellationToken);
        }

        private void ValidateProperties()
        {
            if (string.IsNullOrEmpty(QueueName))
            {
                throw new ConsumerAsyncException("Queue name could not be empty.", nameof(QueueName));
            }

            if (string.IsNullOrEmpty(ExchangeName))
            {
                throw new ConsumerAsyncException("Exchange name could not be empty.", nameof(ExchangeName));
            }

            if (string.IsNullOrEmpty(ConnectionName))
            {
                throw new ConsumerAsyncException("Connectio value could not be empty", nameof(ConnectionName));
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _loggingService.LogInformation($"[{this.GetType().Name}] {ConsumerName} STARTED with the Connection, Exchange, and Queue settings: {ConnectionName} , {ExchangeName} ,{QueueName} ");

            RabbitMQConfig? rabbitMQConfig = _rabbitMQConfig?.GetConfigValue(ConnectionName).Valid();
            RabbitMqExchangeOptions? exchangeConfig = rabbitMQConfig.GetRabbitMqExchangeConfig(ExchangeName).Valid();
            RabbitMqQueueOptions? queueConfig = exchangeConfig.GetQueueConfig(QueueName);

            Connection = _rabbitMQFactory.CreateRabbitMqConnection(rabbitMQConfig);
            Channel = CreateChannel(Connection);

            Channel?.BasicQos(0, PrefetchCount, false);

            var consumer = _rabbitMQFactory.CreateConsumer(Channel);

            consumer.Received += async (_, eventArgs) =>
            {
                try
                {
                    await HandleMessagesAsync(eventArgs, cancellationToken);

                    Channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _loggingService.LogError(ex, $"Error while consuming data from Queue : {QueueName}. Message return to queue");

                    if (!string.IsNullOrEmpty(exchangeConfig?.DeadLetterExchange))
                        Channel.BasicNack(eventArgs.DeliveryTag, false, false);

                    throw;
                }
            };

            _consumerTags = Channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer, consumerTag: ConsumerName);

            return Task.CompletedTask;
        }

        private IModel CreateChannel(IConnection connection)
        {
            connection.CallbackException += HandleConnectionCallbackException;
            if (connection is IAutorecoveringConnection recoveringConnection)
            {
                recoveringConnection.ConnectionRecoveryError += HandleConnectionRecoveryError;
            }

            var channel = connection.CreateModel();
            channel.CallbackException += HandleChannelCallbackException;
            channel.BasicRecoverOk += HandleChannelBasicRecoverOk;
            return channel;
        }

        private void HandleConnectionCallbackException(object sender, CallbackExceptionEventArgs? @event)
        {
            if (@event?.Exception is null)
            {
                return;
            }

            _loggingService.LogError(@event.Exception, @event.Exception.Message);
            throw @event.Exception;
        }

        private void HandleConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs? @event)
        {
            if (@event?.Exception is null)
            {
                return;
            }

            _loggingService.LogError(@event.Exception, @event.Exception.Message);
            throw @event.Exception;
        }

        private void HandleChannelBasicRecoverOk(object sender, EventArgs? @event)
        {
            if (@event is null)
            {
                return;
            }

            _loggingService.LogInformation("Connection has been reestablished");
        }

        private void HandleChannelCallbackException(object sender, CallbackExceptionEventArgs? @event)
        {
            if (@event?.Exception is null)
            {
                return;
            }

            _loggingService.LogError(@event.Exception, @event.Exception.Message);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _loggingService.LogInformation($"[{this.GetType().Name}]  {ConsumerName} STOPED with the Connection, Exchange, and Queue settings: {ConnectionName} , {ExchangeName} ,{QueueName} ");

            Channel?.BasicCancel(_consumerTags);

            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Connection?.Dispose();
                Channel?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
