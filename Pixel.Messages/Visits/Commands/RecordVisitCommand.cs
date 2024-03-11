namespace Pixel.Messages.Visits.Commands;

using Pixel.Shared.Messages;

public class RecordVisitCommand : PixelMessage
{
    public string IpAdress { get; set; } = string.Empty;
    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
}