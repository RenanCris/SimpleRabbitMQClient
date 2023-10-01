using SimpleRabbitMQ.Extensions;
using SimpleRabbitMQ.Services.Interfaces;
using SimpleRabbitMQ.Validation.Infrastructure;
using SimpleRabbitMQ.Validation.Request;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSimpleRabbitMQConfiguration(builder.Configuration)
    .AddConsumerAsync<ConsumerTestAsync>("ConnA", "teste-ex", queueName: "minha-fila-teste", prefetchCount: 10)
    .AddProducerAsync();

var app = builder.Build();

app.MapPost("/producer", async (TestRequest testRequest, IProducingService producer) =>
{
    await producer.SetConnectionName("ConnA").SendStringAsync("TESTE", "teste-ex", "fila");

    return Results.Ok();

}).WithName("producer");

app.AddInicializeSimpleRabbitMQClient();

app.Run();
