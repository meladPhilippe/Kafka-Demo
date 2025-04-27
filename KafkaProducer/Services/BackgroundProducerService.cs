using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using KafkaProducer.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaProducer.Services;

internal class BackgroundProducerService : BackgroundService
{
    private readonly ILogger<BackgroundProducerService> _logger;
    private IProducer<string, UserModel>? _producer;
    public BackgroundProducerService(ILogger<BackgroundProducerService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "http://schema-registry:8081",
                //Url = "http://localhost:8082" // Schema Registry URL 
            };

            var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "kafka:9092"
                //BootstrapServers = "localhost:29092/"
            };

            var avroSerializerConfig = new AvroSerializerConfig
            {
                // optional Avro serializer properties:
                BufferBytes = 100
            };

            using (var producer = new ProducerBuilder<string, UserModel>(producerConfig)
                                     .SetValueSerializer(new AvroSerializer<UserModel>(schemaRegistry, avroSerializerConfig))
                                     .Build())
            {
                int counter = 1; // start counter at 1

                while (!stoppingToken.IsCancellationRequested)
                {

                    UserModel user = new UserModel
                    {
                        Id = counter,
                        Name = $"melad{counter}"
                    };

                    var message = new Message<string, UserModel>
                    {
                        Key = user.Id.ToString(), // Use Id as key
                        Value = user
                    };

                    try
                    {
                        var deliveryResult = await producer.ProduceAsync("Melad", message);
                        _logger.LogInformation($"Produced message to: {deliveryResult.TopicPartitionOffset} | User: {user.Name}");
                    }
                    catch (ProduceException<string, UserModel> ex)
                    {
                        Console.WriteLine($"Error producing message: {ex.Error.Reason}");
                    }

                    counter++; // increment counter after successful produce
                    await Task.Delay(1000, stoppingToken); // delay between messages
                }
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while producing the message.");
        }
    }

    public override void Dispose()
    {
        _producer?.Dispose();
        base.Dispose();
    }
}
