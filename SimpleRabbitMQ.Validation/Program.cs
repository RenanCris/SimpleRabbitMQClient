using SimpleRabbitMQ.Configurations;
using SimpleRabbitMQ.Extensions;
using SimpleRabbitMQ.Services.Interfaces;
using SimpleRabbitMQ.Validation.Infrastructure;
using SimpleRabbitMQ.Validation.Request;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSimpleRabbitMQConfiguration(builder.Configuration)
    .AddConsumerAsync<ConsumerTestAsync>(connectionName: "ConnA", exchangeName: "my-exchange", queueName: "my-queue", prefetchCount: 10)
    .AddProducerAsync(new OutBoxConfig() 
    { 
        ConnectionStringDataBase = builder.Configuration.GetConnectionString("OutBoxConnection"),
        CronJobConfig = "0 0/1 * * * ?"
    });

var app = builder.Build();

app.MapPost("/producer", async (TestRequest testRequest, IProducingMessageService producer) =>
{
    await producer
                .SetConnectionName("ConnA")
                .SendStringAsync(JsonSerializer.Serialize(testRequest), "teste-ex", "fila");

    return Results.Ok();

}).WithName("producer");

app.AddInicializeSimpleRabbitMQClient();

app.Run();
