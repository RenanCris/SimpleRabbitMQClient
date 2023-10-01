using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Extensions
{
    /// <summary>
    /// BasicDeliverEventArgsExtensions extension that help to work with messages,
    /// </summary>
    public static class BasicDeliverEventArgsExtensions
    {
        /// <summary>
        /// Get message from BasicDeliverEventArgs body.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <returns>Message as a string.</returns>
        public static string GetMessage(this BasicDeliverEventArgs eventArgs)
        {
            eventArgs.EnsureIsNotNull();
            return Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        }

        /// <summary>
        /// Get message payload.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <typeparam name="T">Type of a message body.</typeparam>
        /// <returns>Object of type <see cref="T"/>.</returns>
        public static T? GetPayload<T>(this BasicDeliverEventArgs eventArgs)
        {
            eventArgs.EnsureIsNotNull();
            var messageString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonSerializer.Deserialize<T>(messageString);
        }
       
        private static BasicDeliverEventArgs EnsureIsNotNull(this BasicDeliverEventArgs eventArgs)
        {
            if (eventArgs is null)
            {
                throw new ArgumentNullException(nameof(eventArgs), "BasicDeliverEventArgs have to be not null to parse a message");
            }

            return eventArgs;
        }
    }
}
