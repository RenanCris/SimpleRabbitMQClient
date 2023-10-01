namespace SimpleRabbitMQ.Exceptions
{
    internal class ConsumerAsyncException : Exception 
    {
        public string PropertyName { get; }
        public ConsumerAsyncException()
        {
        }

        public ConsumerAsyncException(string? message) : base(message)
        {
        }

        public ConsumerAsyncException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public ConsumerAsyncException(string? message, string propertyName) : base(message)
        {
            PropertyName = propertyName;
        }
    }
}
