using KafkaConsumer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

// Add services to the container.
builder.ConfigureServices((hostContext, services) =>
{
    services.AddHostedService<BackgroundConsumerService>(); //  Consumer Service
});

var app = builder.Build();
await app.RunAsync();