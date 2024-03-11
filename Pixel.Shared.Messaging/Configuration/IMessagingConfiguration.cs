namespace Pixel.Shared.Messaging.Configuration;

public interface IMessagingConfiguration
{
    string? Host { get; }
    string? VirtualHost { get; }
    string? User { get; }
    string? Password { get; }
}