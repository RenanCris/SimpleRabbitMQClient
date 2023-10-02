# SimpleRabbitMQClient
Package with a simple RabbitMQ client, with the option to use the outbox pattern. 

Developed in .NET 6, it is a RabbitMQ client package, built upon the rabbitMQClient.net foundation. It provides semantic routing with the possibility of declarative configuration of various connection types, exchanges, and queues automatically. It allows registration of multiple consumers and direct producers for RabbitMQ, following the outbox pattern if needed.

# Package
	NuGet\Install-Package SimpleRabbitMQClient -Version 1.0.1

# Usage

In your class Program.cs add this code. Principal Configuration

    builder.Services
    .AddSimpleRabbitMQConfiguration(builder.Configuration)

Consumer Example:

    builder.Services
    .AddSimpleRabbitMQConfiguration(builder.Configuration)
    .AddConsumerAsync<ConsumerTestAsync>(connectionName: "ConnA", exchangeName: "my-exchange", queueName: "my-queue", prefetchCount: 10)

Producer Example (Without Outbox Patten):

    builder.Services
    .AddSimpleRabbitMQConfiguration(builder.Configuration)
    .AddProducerAsync()


To use the direct publishing service for RabbitMQ.

    IProducingMessageService


Producer Example (With Outbox Patten):

    builder.Services
    .AddSimpleRabbitMQConfiguration(builder.Configuration)
    .AddProducerAsync(new OutBoxConfig() 
    { 
        ConnectionStringDataBase = builder.Configuration.GetConnectionString("OutBoxConnection"),
        CronJobConfig = "0 0/1 * * * ?"
    })

To use the indirect publishing service. Persistence message in DataBase

    IProducingOutBoxService

# Appssetings Configuration

To use the component, it is necessary to provide RabbitMQ configurations, and if you wish to use the production strategy based on the OutBox pattern, you will also need to configure a connection string to the database with MySQL as the provider. Below are the configurations


    "RabbitMQConfiguration": {
    "LogEnabled": true,
    "RabbitMQConfig": [
      {
        "HostName": "localhost", // required
        "Port": 5672,            // required
        "Name": "ConnA",         // required
        "Exchanges": [
          {
            "Name": "my-exchange",  // required
            "DeadLetterExchange": "my-exchange-dead-letter",
            "Type": "direct",
            "Durable": true,
            "AutoDelete": false,
            "DeadLetterExchangeType": "direct",
            "Arguments": {"",""},
            "Queues": [
              {
                "Name": "my-queue", // required
                "RoutingKey": "queue-rk",
                "Durable": false,
                "Exclusive": false,
                "AutoDelete": false,
                "Arguments": {"",""},
              }
            ]
          }
        ]
      }
    ]
  }


# Consumer Example

    // ConsumerAsyncJsonObjectBase => Deserialize the message into an object for business processing.

    public class ConsumerTestAsync : ConsumerAsyncJsonObjectBase<AccountMessage>
    {
        public ConsumerTestAsync(ILoggingService loggingService, 
            IRabbitMQFactory rabbitMQFactory, 
            IOptions<RabbitMQConfiguration> rabbitMQConfig
            , string connectionName
            , string exchangeName
            , string queueName
            , ushort prefetchCount = 0) : base(loggingService, rabbitMQFactory, rabbitMQConfig, connectionName, exchangeName, queueName, prefetchCount)
        {
        }

        protected override Task HandleMessageClientAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken)
        {
            _loggingService.LogInformation(MessageObject.Id.ToString());

            return Task.CompletedTask;
        }
    }


    // ConsumerAsyncBase => Get message String 

    public class ConsumerTestAsync : ConsumerAsyncBase
    {
        public ConsumerTestAsync(ILoggingService loggingService, 
            IRabbitMQFactory rabbitMQFactory, 
            IOptions<RabbitMQConfiguration> rabbitMQConfig
            , string connectionName
            , string exchangeName
            , string queueName
            , ushort prefetchCount = 0) : base(loggingService, rabbitMQFactory, rabbitMQConfig, connectionName, exchangeName, queueName, prefetchCount)
        {
        }

        protected override Task HandleMessageClientAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken)
        {
            var _messageString = message.GetMessage();
            return Task.CompletedTask;
        }
    }


    
