namespace Pixel.Shared.Messages;

public interface IPixelMessage
{
    Ulid MessageId { get; }
    DateTime UtcTimeStamp { get; }
}