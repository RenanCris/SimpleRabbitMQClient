using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Extensions;
using SimpleRabbitMQ.Factories;
using SimpleRabbitMQ.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleRabbitMQ.Services.Producers
{
    internal sealed class ProducerAsync : IHostedService
    {
        private readonly ILoggingService _loggingService;
        private readonly IRabbitMQFactory _rabbitMQFactory;
        private readonly RabbitMQConfiguration _rabbitMQConfig;
        private readonly IProducingMessageService _producingService;

        public ProducerAsync(ILoggingService loggingService,
            IRabbitMQFactory rabbitMQFactory,
            IOptions<RabbitMQConfiguration> rabbitMQConfig,
            IProducingMessageService producingService)
        {
            _loggingService = loggingService;
            _rabbitMQFactory = rabbitMQFactory;
            _rabbitMQConfig = rabbitMQConfig.Value;
            _producingService = producingService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _loggingService.LogInformation($"[ProducerAsync] PRODUCER STARTED");

            foreach (var rabbitMQ in _rabbitMQConfig.RabbitMQConfig)
            {
                try
                {
                    var connection = _rabbitMQFactory.CreateRabbitMqConnection(rabbitMQ);
                    var channel = connection?.CreateModel();

                    if (connection is null || channel is null) 
                    {
                        _loggingService.LogInformation($"[ProducerAsync] Not Created connection or channel");
                        continue;
                    }

                    _producingService.UseDataConnection(connection, channel, rabbitMQ.Name);
                }
                catch (Exception ex)
                {
                    _loggingService.LogError(ex, $"[ProducerAsync] error! {rabbitMQ.Name}");
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _loggingService.LogInformation($"[ProducerAsync] PRODUCER STOPED");

            return Task.CompletedTask;
        }


    }
}
