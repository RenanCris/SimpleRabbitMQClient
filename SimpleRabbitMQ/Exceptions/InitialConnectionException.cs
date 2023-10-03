using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Exceptions
{
    internal class InitialConnectionException : Exception
    {
        public InitialConnectionException()
        {
        }

        public InitialConnectionException(string? message) : base(message)
        {
        }

        public InitialConnectionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
