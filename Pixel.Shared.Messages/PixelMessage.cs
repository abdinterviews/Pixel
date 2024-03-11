namespace Pixel.Shared.Messages;

using System;

public class PixelMessage : IPixelMessage
{
    public PixelMessage()
    {
        MessageId = Ulid.NewUlid();
        UtcTimeStamp = DateTime.UtcNow;
    }

    public Ulid MessageId { get; }

    public DateTime UtcTimeStamp { get; }
}