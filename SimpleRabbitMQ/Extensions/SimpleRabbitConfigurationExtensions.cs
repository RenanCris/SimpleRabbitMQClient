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
using SimpleRabbitMQ.Services.Producers;
using SimpleRabbitMQ.Repository;
using SimpleRabbitMQ.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using SimpleRabbitMQ.Jobs;
using Microsoft.Extensions.Logging;
using static Quartz.Logging.OperationName;

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
        public static ISimpleRabbitMQConfigBuilder AddProducerAsync(this ISimpleRabbitMQConfigBuilder services, OutBoxConfig outBoxConfig = null) 
        {
            services.Services.TryAddSingleton<IProducingMessageService, ProducingMessageService>();
            services.Services.AddHostedService<ProducerAsync>();

            if (outBoxConfig is not null)
                EnableOutboxService(services, outBoxConfig.ConnectionStringDataBase, outBoxConfig.CronJobConfig);

            return services;
        }

        /// <summary>
        /// Register services outbox pattern."
        /// </summary>
        /// 

        private static void EnableOutboxService(ISimpleRabbitMQConfigBuilder services, string connectionString, string cronConfig = "0 0/1 0 ? * * *")
        {
            ValidateEnableOutboxService(connectionString);

            services.Services.AddDbContext<OutboxMessageDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }, ServiceLifetime.Scoped);

            services.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
            services.Services.AddScoped<IProducingOutBoxService, ProducingOutBoxService>();

            services.Services.AddSingleton(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().Result;
                return scheduler;
            });

            services.Services.AddQuartz(options =>
            {
                options.AddJob<OutBoxJob>(group => group
                    .WithIdentity("outboxjob").Build()
                    );

                options.AddTrigger(trigger => trigger
                    .WithIdentity("outboxjob-trigger")
                    .ForJob("outboxjob")
                    .WithCronSchedule(cronConfig)
                    );
            });

            services.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }

        private static void ValidateAddConsumer<TConsumer>(string connectionName, string exchangeName) 
        {
            if (string.IsNullOrEmpty(connectionName) && string.IsNullOrEmpty(exchangeName)) 
            {
                throw new ArgumentException($"The connection and exchange names were not specified. {nameof(TConsumer)}");
            };
        }

        private static void ValidateEnableOutboxService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"The connection string were not specified.");
            };
        }
    }
}
