using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Pixel.Http;
using Pixel.Shared.Messaging.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Setup configuration
var configuration = builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("secrets.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

builder.Services.AddSingleton<IMessagingConfiguration>((IServiceProvider context) =>
{
    var messagingConfiguration = configuration.GetRequiredSection("Messaging").Get<MessagingConfiguration>();

    return messagingConfiguration ?? throw new InvalidOperationException("Failed to load Messaging configuration");
});

// Setup logging
builder.Logging
    .ClearProviders()
    .AddConsole();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// Configure Mass Transit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var messagingConfig = context.GetRequiredService<IMessagingConfiguration>();

        cfg.Host(
            messagingConfig.Host, 
            messagingConfig.VirtualHost, 
            h => {
                h.Username(messagingConfig.User);
                h.Password(messagingConfig.Password);
            }
        );

        cfg.ConfigureEndpoints(context);
    });
});

// Build container
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Actions
app.MapGroup(string.Empty)
   .MapPixelApi();

// Start app
app.Run();