using RabbitMQ.Client;
using SimpleRabbitMQ.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Services.Producers
{
    internal class ProducingMessageService : IProducingMessageService, IDisposable
    {
        private object _lock = new();

        private readonly Dictionary<string, Tuple<IConnection, IModel>> _configsConnection;
        private string ConnectionNameUsed = string.Empty;

        public ProducingMessageService()
        {
            _configsConnection = new Dictionary<string, Tuple<IConnection, IModel>>();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var config in _configsConnection)
            {
                var (connection, chanel) = config.Value;

                if (chanel?.IsOpen == true)
                {
                    chanel.Close((int)HttpStatusCode.OK, "Channel closed");
                }

                if (connection?.IsOpen == true)
                {
                    connection.Close();
                }

                chanel?.Dispose();
                connection?.Dispose();
            }
        }

        public void UseDataConnection(IConnection connection, IModel channel, string connectionName)
        {
            _configsConnection.Add(connectionName, Tuple.Create(connection, channel));
        }

        private IModel GetChanel() => _configsConnection.FirstOrDefault(x => x.Key == ConnectionNameUsed).Value.Item2;

        /// <inheritdoc/>
        public void Send<T>(T @object, string exchangeName, string routingKey = "") where T : class
        {
            EnsureProducingChannelAndConnectionNameIsNotNull();
            ValidateArguments(exchangeName);
            var json = JsonSerializer.Serialize(@object);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        /// <inheritdoc/>
        public void SendJson(string json, string exchangeName, string routingKey = "")
        {
            EnsureProducingChannelAndConnectionNameIsNotNull();
            ValidateArguments(exchangeName);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        /// <inheritdoc/>
        public void SendString(string message, string exchangeName, string routingKey = "")
        {
            EnsureProducingChannelAndConnectionNameIsNotNull();
            ValidateArguments(exchangeName);
            var bytes = Encoding.UTF8.GetBytes(message);
            Send(bytes, CreateProperties(), exchangeName, routingKey);
        }

        /// <inheritdoc/>
        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey = "") where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey));

        /// <inheritdoc/>
        public async Task SendJsonAsync(string json, string exchangeName, string routingKey = "") =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey));

        /// <inheritdoc/>
        public async Task SendStringAsync(string message, string exchangeName, string routingKey = "") =>
            await Task.Run(() => SendString(message, exchangeName, routingKey));

        private void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            EnsureProducingChannelAndConnectionNameIsNotNull();
            ValidateArguments(exchangeName);
            lock (_lock)
            {
                GetChanel().BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        private IBasicProperties CreateProperties()
        {
            var properties = GetChanel().CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }

        private IBasicProperties CreateJsonProperties()
        {
            var properties = GetChanel().CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            return properties;
        }

        private void EnsureProducingChannelAndConnectionNameIsNotNull()
        {
            if (string.IsNullOrEmpty(ConnectionNameUsed))
                throw new ArgumentException($"Producing connection name is null. Configure {nameof(IProducingMessageService)} or full functional {nameof(IProducingMessageService)} for producing messages");

            if (GetChanel() is null)
            {
                throw new ArgumentException($"Producing channel is null. Configure {nameof(IProducingMessageService)} or full functional {nameof(IProducingMessageService)} for producing messages");
            }
        }

        internal void ValidateArguments(string exchangeName)
        {
            if (string.IsNullOrEmpty(exchangeName))
            {
                throw new ArgumentException($"Argument {nameof(exchangeName)} is null or empty.", nameof(exchangeName));
            }
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

        public string GetConnectionNameUsed() => this.ConnectionNameUsed;
    }
}
