using Microsoft.Extensions.Logging;
using SimpleRabbitMQ.Factories;
using SimpleRabbitMQ.Repository;
using SimpleRabbitMQ.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleRabbitMQ.Services.Producers
{
    internal class ProducingOutBoxService : IProducingOutBoxService
    {
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        private readonly ILogger<ProducingOutBoxService> _logger;
        private string ConnectionNameUsed = string.Empty;

        public ProducingOutBoxService(
            ILogger<ProducingOutBoxService> logger,
            IOutboxMessageRepository outboxMessageRepository)
        {
            _logger = logger;
            _outboxMessageRepository = outboxMessageRepository;
        }

        public IProducingBaseService SetConnectionName(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new ArgumentException($"Argument {nameof(connectionName)} is null or empty.", nameof(connectionName));
            }

            ConnectionNameUsed = connectionName;
            return this;
        }

        public void Send<T>(T @object, string exchangeName, string routingKey = "") where T : class
        {
            try
            {
                _outboxMessageRepository.Insert(OutboxMessageFactory.CreateByObject(ConnectionNameUsed, exchangeName, routingKey, @object));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProducingService] Error while publishing message.");
                throw;
            }
        }

        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey = "") where T : class
        {
            try
            {
                await _outboxMessageRepository.InsertAsync(OutboxMessageFactory.CreateByObject(ConnectionNameUsed, exchangeName, routingKey, @object));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProducingService] Error while publishing message.");
                throw;
            }
        }

        public void SendJson(string json, string exchangeName, string routingKey = "")
        {
            try
            {
                _outboxMessageRepository.Insert(OutboxMessageFactory.Create(ConnectionNameUsed, exchangeName, routingKey, json));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProducingService] Error while publishing message.");
                throw;
            }
        }

        public async Task SendJsonAsync(string json, string exchangeName, string routingKey = "")
        {
            try
            {
                await _outboxMessageRepository.InsertAsync(OutboxMessageFactory.Create(ConnectionNameUsed, exchangeName, routingKey, json));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProducingService] Error while publishing message.");
                throw;
            }
        }

        public void SendString(string message, string exchangeName, string routingKey = "")
        {
            try
            {
                _outboxMessageRepository.Insert(OutboxMessageFactory.Create(ConnectionNameUsed, exchangeName, routingKey, message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProducingService] Error while publishing message.");
                throw;
            }
        }

        public async Task SendStringAsync(string message, string exchangeName, string routingKey = "")
        {
            try
            {
                await _outboxMessageRepository.InsertAsync(OutboxMessageFactory.Create(ConnectionNameUsed, exchangeName, routingKey, message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProducingService] Error while publishing message.");
                throw;
            }
        }

    }
}
