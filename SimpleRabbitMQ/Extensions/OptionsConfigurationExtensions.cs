using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimpleRabbitMQ.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Extensions
{
    internal static class OptionsConfigurationExtensions
    {
        public static IServiceCollection AddSimpleOptions(this IServiceCollection services, IConfiguration config ) 
        {
            services.Configure<RabbitMQConfiguration>(config.GetSection("RabbitMQConfiguration"));

            return services;
        }
    }
}
