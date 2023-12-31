﻿using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Extensions;
using SimpleRabbitMQ.Factories;
using SimpleRabbitMQ.Services;
using SimpleRabbitMQ.Services.Interfaces;
using SimpleRabbitMQ.Validation.Message;

namespace SimpleRabbitMQ.Validation.Infrastructure
{
    public class ConsumerTestAsync : ConsumerAsyncBase
    {
        public ConsumerTestAsync(ILoggingService loggingService, 
            IRabbitMQFactory rabbitMQFactory, 
            IOptions<RabbitMQConfiguration> rabbitMQConfig
            , string connectionName
            , string exchangeName
            , string queueName
            , ushort prefetchCount = 0) : base(loggingService, rabbitMQFactory, rabbitMQConfig, connectionName, exchangeName, queueName, prefetchCount)
        {
        }

        protected override Task HandleMessageClientAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken)
        {
            var _messageString = message.GetMessage();
            return Task.CompletedTask;
        }
    }
}
