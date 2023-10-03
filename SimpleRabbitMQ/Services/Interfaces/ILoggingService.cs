using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Services.Interfaces
{
    public interface ILoggingService
    {/// <summary>
     /// Log occured error.
     /// </summary>
        void LogError(Exception exception, string message);

        /// <summary>
        /// Log warning.
        /// </summary>
        void LogWarning(string message);

        /// <summary>
        /// Log information.
        /// </summary>
        void LogInformation(string message);

        /// <summary>
        /// Log debug.
        /// </summary>
        void LogDebug(string message);
    }
}
