using Microsoft.Extensions.DependencyInjection;

namespace SimpleRabbitMQ.Registrations
{
    internal class SimpleRabbitMQConfigBuilder : ISimpleRabbitMQConfigBuilder
    {
        private readonly IServiceCollection _services;
        public SimpleRabbitMQConfigBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public IServiceCollection Services => _services;
    }
}
