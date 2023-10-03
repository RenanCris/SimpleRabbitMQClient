using Microsoft.OpenApi.Models;
using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Extensions;
using SimpleRabbitMQ.Services.Interfaces;
using SimpleRabbitMQ.Validation.Extensions;
using SimpleRabbitMQ.Validation.Infrastructure;
using SimpleRabbitMQ.Validation.Request;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Producers", Version = "v1" });
});

builder.Services
    .AddSimpleRabbitMQConfiguration(builder.Configuration)
    .AddConsumerAsync<ConsumerTestAsync>(connectionName: "ConnA", exchangeName: "my-exchange", queueName: "my-queue", prefetchCount: 10)
    .AddConsumerAsync<ConsumerTestAsync>(connectionName: "ConnA", exchangeName: "my-exchange", queueName: "my-queue_2", prefetchCount: 2)
    .AddProducerAsync(new OutBoxConfig() 
    { 
        ConnectionStringDataBase = builder.Configuration.GetConnectionString("OutBoxConnection"),
        CronJobConfig = "0 0/1 * * * ?"
    });

var app = builder.Build();

app.AddRoutes();
app.AddInicializeSimpleRabbitMQClient();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Producers V1");
});

app.Run();
