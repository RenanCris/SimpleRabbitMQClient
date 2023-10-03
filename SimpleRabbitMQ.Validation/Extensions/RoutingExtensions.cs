using SimpleRabbitMQ.Services.Interfaces;
using SimpleRabbitMQ.Validation.Request;

namespace SimpleRabbitMQ.Validation.Extensions
{
    public static class RoutingExtensions
    {
        public static WebApplication AddRoutes(this WebApplication app) 
        {
            app.MapPost("/producer/send-direct-rabbitmq", async (AccountRequest accountRequest, IProducingMessageService producer) =>
            {
                await producer
                            .SetConnectionName("ConnA")
                            .SendAsync(accountRequest, "my-exchange", "queue-rk");

                return Results.Ok();

            }).WithName("producer rabbitmq");

            app.MapPost("/producer/send-using-outbox", async (AccountRequest accountRequest, IProducingOutBoxService producer) =>
            {
                await producer
                            .SetConnectionName("ConnA")
                            .SendAsync(accountRequest, "my-exchange", "queue-rk_2");

                return Results.Ok();

            }).WithName("producer outbox");

            return app;
        }
    }
}
