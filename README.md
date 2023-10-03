# SimpleRabbitMQClientOutBox
Package with a simple RabbitMQ client, with the option to use the outbox pattern. 

Developed in .NET 6, it is a RabbitMQ client package, built upon the rabbitMQClient.net foundation. It provides semantic routing with the possibility of declarative configuration of various connection types, exchanges, and queues automatically. It allows registration of multiple consumers and direct producers for RabbitMQ, following the outbox pattern if needed.

# Package
	NuGet\Install-Package SimpleRabbitMQClientOutBox -Version 1.0.3

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

**It is crucial to specify the connection name, which is equivalent to the 'Name' property in the 'RabbitMQConfig' object**


    await producer
                .SetConnectionName("ConnA")
                .SendAsync(Object, "test-exchange", "queue-RoutingKey");

    //appssetings config

    "RabbitMQConfig":  [{
      "HostName": "localhost", 
      "Port": 5672,            
      "Name": "ConnA" // Connection Name
    }]           


# Appssetings Configuration

To use the component, it is necessary to provide RabbitMQ configurations, and if you wish to use the production strategy based on the OutBox pattern, you will also need to configure a connection string to the database with MySQL as the provider. Below are the configurations


    "RabbitMQConfiguration": {
    "LogEnabled": true,
    "RabbitMQConfig": [
      {
        "HostName": "localhost", // required
        "Port": 5672,            // required
        "Name": "ConnA",         // required (ConnectionName) Important
        "InitialConnectionRetries": 3,
        "InitialConnectionRetryTimeoutMilliseconds" : 200,
        "RequestedConnectionTimeout": 1, 
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
  

# Automatic initialization

  **Important!!!**

    app.AddInicializeSimpleRabbitMQClient();

# Consumer Example

    // ConsumerAsyncJsonObjectBase => Deserialize the message into an object for business processing.
    
    // The data representation of the object is contained in the protected property 'MessageObject' with a private setter within the class.


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

# For Data Base OutBox Usage

The database connection provider is MySQL. To find the script for creating the database and table, please check the **'script.txt'** file in the root of the GitHub repository.

# RabbitMQ Config Inicialization

![exchange](https://github.com/RenanCris/SimpleRabbitMQClient/assets/7238977/4d47d6ae-31ec-4fc7-9682-2e1e8096132f)
![queu](https://github.com/RenanCris/SimpleRabbitMQClient/assets/7238977/9887eb0d-018e-4265-bdef-4e5599cbb590)
![queu-config](https://github.com/RenanCris/SimpleRabbitMQClient/assets/7238977/9699a90d-36f3-4bb8-8bdd-8049515585cd)


# Logs

![log](https://github.com/RenanCris/SimpleRabbitMQClient/assets/7238977/af851a12-96cf-4eec-9f57-ea6c7edbc292)

    
# Example Test

*This file* : **docker-compose.yml**

![swagger](https://github.com/RenanCris/SimpleRabbitMQClient/assets/7238977/f2c41d72-56c2-47df-a34e-cea14e179fc3)

# DataBase 

![bd](https://github.com/RenanCris/SimpleRabbitMQClient/assets/7238977/8cf0a848-4eba-44fe-9927-6065936c142f)

## Etiquetas

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)


