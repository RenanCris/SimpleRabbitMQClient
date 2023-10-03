namespace SimpleRabbitMQ.Exceptions
{
    internal class ConsumerAsyncException : Exception 
    {
        public string PropertyName { get; }

        public ConsumerAsyncException(string? message, string propertyName) : base(message)
        {
            PropertyName = propertyName;
        }
    }
}
