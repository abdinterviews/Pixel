using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pixel.Data.Adapters.Visits;
using Pixel.Domain.Core.Visits.Ports;
using Pixel.Storage.Service.Visits.Consumers;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("secrets.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RecordVisitCommandConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddConfigureEndpointsCallback((name, cfg) =>
    {
        cfg.UseMessageRetry(r => r.Immediate(2));
        cfg.PublishFaults = true;
    });
});

// Registering Adapters
builder.Services.AddScoped<IVisitRecorder, VisitRecorder>();

var app = builder.Build();
app.Run();