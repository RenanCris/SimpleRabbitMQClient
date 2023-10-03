namespace SimpleRabbitMQ.Configurations
{
    public class RabbitMqQueueOptions 
    {
        /// <summary>
        /// Queue name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Durable option.
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// Exclusive option.
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// AutoDelete option.
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// Routing keys collection that queue "listens".
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// Additional arguments.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
       
    }
}
