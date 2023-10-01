using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Services
{
    internal class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly bool loggingEnabled;

        public LoggingService (
            ILogger<LoggingService> logger,
            IOptions<RabbitMQConfiguration> options)
        {
            _logger = logger;
            loggingEnabled = options.Value.LogEnabled;
        }

        public void LogError(Exception exception, string message)
        {
            _logger.LogError(new EventId(), exception, message);
        }

        public void LogWarning(string message)
        {
            if (!loggingEnabled)
            {
                return;
            }

            _logger.LogWarning(message);
        }

        /// <inheritdoc />
        public void LogInformation(string message)
        {
            if (!loggingEnabled)
            {
                return;
            }

            _logger.LogInformation(message);
        }

        /// <inheritdoc />
        public void LogDebug(string message)
        {
            if (!loggingEnabled)
            {
                return;
            }

            _logger.LogDebug(message);
        }
    }
}
