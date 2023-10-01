namespace SimpleRabbitMQ.Configurations
{
    public class RabbitMqTcpEndpoint 
    {
        /// <summary>
        /// RabbitMQ server.
        /// </summary>
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// Tcp connection port.
        /// </summary>
        public int Port { get; set; } = 5672;

        /// <summary>
        /// Ssl option.
        /// </summary>
        /// <remarks>
        /// Should be null if certificate should not be used.
        /// </remarks>
        public RabbitMqSslOption? SslOption { get; set; }
    }
}
