using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Quartz;
using SimpleRabbitMQ.Repository;
using SimpleRabbitMQ.Services.Interfaces;
using SimpleRabbitMQ.Services.Producers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Jobs
{
    internal class OutBoxJob : IJob
    {
        private readonly IProducingMessageService _producingMessageService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutBoxJob> _logger;

        public OutBoxJob(
            IProducingMessageService producingMessageService,
            IServiceProvider serviceProvider,
            ILogger<OutBoxJob> logger)
        {
            _producingMessageService = producingMessageService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope()) 
            {
                var _outboxMessageRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
                var allPending = await _outboxMessageRepository.GetAllPendingAsync();

                foreach (var outboxMessage in allPending)
                {
                    try
                    {
                        outboxMessage.UpdateProcess();
                        await _outboxMessageRepository.UpdateAsync(outboxMessage);

                        await _producingMessageService
                           .SetConnectionName(outboxMessage.ConnectionName)
                           .SendStringAsync(outboxMessage.MessageData, outboxMessage.ExchangeName, outboxMessage.RoutingKey);
                    }
                    catch (MySqlException ex)
                    {
                        _logger.LogError(ex, $"[OutBoxJob] MySqlException , Message ID {outboxMessage.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"[OutBoxJob] error send message rabbitMQ, Message ID {outboxMessage.Id}");
                    }
                }
            }
        }
    }
}
