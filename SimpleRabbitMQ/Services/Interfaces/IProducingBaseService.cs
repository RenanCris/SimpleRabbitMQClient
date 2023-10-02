using RabbitMQ.Client;

namespace SimpleRabbitMQ.Services.Interfaces
{
    public interface IProducingBaseService 
    {
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send<T>(T @object, string exchangeName, string routingKey = "") where T : class;


        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void SendJson(string json, string exchangeName, string routingKey = "");

       
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void SendString(string message, string exchangeName, string routingKey = "");
       

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendAsync<T>(T @object, string exchangeName, string routingKey = "") where T : class;


        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendJsonAsync(string json, string exchangeName, string routingKey = "");

       
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendStringAsync(string message, string exchangeName, string routingKey = "");

        IProducingBaseService SetConnectionName(string connectionName);

    }
}
