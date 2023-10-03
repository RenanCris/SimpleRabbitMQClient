using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Configurations
{
    public class RabbitMQConfiguration
    {
        /// <summary>
        /// The main class that represents the configuration of the RabbitMQ client is typically called
        /// </summary>
        public RabbitMQConfig[] RabbitMQConfig { get; set; } = new RabbitMQConfig[0];

        /// <summary>
        /// Enables logging of information and warnings. Error logs are always written.
        /// </summary>
        public bool LogEnabled { get; set; } = true;

    }
}
