namespace Pixel.Shared.Messaging.Configuration;

public class MessagingConfiguration : IMessagingConfiguration
{
    public string? Host { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? VirtualHost { get; set; }
}