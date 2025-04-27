using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using KafkaConsumer.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Confluent.Kafka.SyncOverAsync;

namespace KafkaConsumer.Services;

internal class BackgroundConsumerService : BackgroundService
{
    private readonly IConsumer<string, UserModel> _consumer;
    private readonly ILogger<BackgroundConsumerService> _logger;

    public BackgroundConsumerService(ILogger<BackgroundConsumerService> logger)
    {
        _logger = logger;

        var consumerConfig = new ConsumerConfig
        {
            //BootstrapServers = "localhost:29092/",
            BootstrapServers = "kafka:9092",
            GroupId = "melad-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
           // Url = "http://localhost:8082" // Schema Registry URL 
            Url = "http://schema-registry:8081"
        };

        var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

        _consumer = new ConsumerBuilder<string, UserModel>(consumerConfig)
            .SetValueDeserializer(new AvroDeserializer<UserModel>(schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();

        _consumer.Subscribe("Melad");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Poll for messages, respect the cancellation token
                var consumeResult = await Task.Run(() => _consumer.Consume(stoppingToken), stoppingToken);

                if (consumeResult != null)
                {
                    var user = consumeResult.Message.Value;
                    _logger.LogInformation($"Consumed User: {user.Id} - {user.Name}");
                }
                else
                {
                    // If no message is received, wait a little to prevent tight CPU loop
                    await Task.Delay(100, stoppingToken); // Wait for 100ms before trying again
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"Consume error: {ex.Error.Reason}");
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully when the task is stopped
                _logger.LogInformation("Consumer operation was canceled.");
            }
            catch (Exception ex)
            {
                // Handle any other general exceptions
                _logger.LogError($"Unexpected error: {ex.Message}");
            }
        }
    }


    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}
