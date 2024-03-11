namespace Pixel.Domain.Core.Visits.Entities;

public class Visit
{
    public Ulid Id { get; set; } = new Ulid();
    public DateTime UtcTimeStamp { get; set; } = DateTime.UtcNow;
    public string IpAdress { get; set; } = string.Empty;
    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
}