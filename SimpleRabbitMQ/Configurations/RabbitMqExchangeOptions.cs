namespace SimpleRabbitMQ.Configurations
{
    public class RabbitMqExchangeOptions 
    {

        /// <summary>
        /// Exchange type.
        /// </summary>
        public string Type { get; set; } = "direct";

        /// <summary>
        /// Durable option.
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// AutoDelete option.
        /// </summary>
        public bool AutoDelete { get; set; }
       

        /// <summary>
        /// Dead-letter-exchange type.
        /// </summary>
        public string DeadLetterExchangeType { get; set; } = "direct";

        /// Additional arguments.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Collection of queues bound to the exchange.
        /// </summary>
        public IList<RabbitMqQueueOptions> Queues { get; set; } = new List<RabbitMqQueueOptions>();

        /// <summary>
        /// Exchange name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of dead-letter-exchange.
        /// </summary>
        public string DeadLetterExchange { get; set; }
    }
}
