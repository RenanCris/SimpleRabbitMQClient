using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Factories;
using SimpleRabbitMQ.Registrations;
using SimpleRabbitMQ.Registrations.Builders;
using SimpleRabbitMQ.Services;
using SimpleRabbitMQ.Services.Interfaces;
using System.Threading.Channels;
using System;

namespace SimpleRabbitMQ.Extensions
{
    public static class SimpleRabbitConfigurationExtensions
    {
        /// <summary>
        /// Initialization services for the Simple RabbitMQ Client services.
        /// </summary>
        /// <returns>Object of type SimpleRabbitMQConfigBuilder />.</returns>

        public static ISimpleRabbitMQConfigBuilder AddSimpleRabbitMQConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.TryAddSingleton<ILoggingService, LoggingService>();
            services.AddSimpleOptions(configuration);
            services.TryAddSingleton<IRabbitMQFactory, RabbitMQFactory>();
            services.TryAddSingleton<IProducingService, ProducingService>();
            services.TryAddSingleton<IInicializeConfiguration, InicializeConfiguration>();

            return new SimpleRabbitMQConfigBuilder(services);
        }

        /// <summary>
        /// Creates all the queue, exchange, binds, and dead letter configurations as indicated in the appsettings.
        /// </summary>

        public static IApplicationBuilder AddInicializeSimpleRabbitMQClient(this IApplicationBuilder app)
        {
            IServiceProvider serviceProvider = app.ApplicationServices;

            var inicializeConfig = serviceProvider.GetService<IInicializeConfiguration>();

            inicializeConfig?.Builder();

            return app;
        }

        /// <summary>
        /// Register consumers for queues."
        /// </summary>
        /// 
        public static ISimpleRabbitMQConfigBuilder AddConsumerAsync<TConsumer>(this ISimpleRabbitMQConfigBuilder services, string connectionName, string exchangeName, string queueName, ushort prefetchCount = 0)
            where TConsumer : IConsumerServiceAsync
        {
            ValidateAddConsumer<TConsumer>(connectionName, exchangeName);

            services.Services.AddSingleton(typeof(IHostedService), sp =>
            {
                return (TConsumer)Activator.CreateInstance(typeof(TConsumer),
                    sp.GetRequiredService<ILoggingService>(),
                    sp.GetRequiredService<IRabbitMQFactory>(),
                    sp.GetService<IOptions<RabbitMQConfiguration>>(),
                    connectionName,
                    exchangeName,
                    queueName,
                    prefetchCount);
            });

            return services;
        }

        /// <summary>
        /// Register producers for exchange."
        /// </summary>
        /// 
        public static ISimpleRabbitMQConfigBuilder AddProducerAsync(this ISimpleRabbitMQConfigBuilder services) 
        {
            services.Services.AddHostedService<ProducerAsync>();

            return services;
        }

        private static void ValidateAddConsumer<TConsumer>(string connectionName, string exchangeName) 
        {
            if (string.IsNullOrEmpty(connectionName) && string.IsNullOrEmpty(exchangeName)) 
            {
                throw new ArgumentException($"The connection and exchange names were not specified. {nameof(TConsumer)}");
            };
        }
    }
}
