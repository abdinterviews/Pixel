namespace Pixel.Data.Adapters.Visits;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pixel.Domain.Core.Visits.Entities;
using Pixel.Domain.Core.Visits.Ports;

public class VisitRecorder : IVisitRecorder
{
    //Only 1 thread can access resource
    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(initialCount: 1);

    private readonly string outputFilePath;

    private readonly ILogger<VisitRecorder> logger;

    public VisitRecorder(ILogger<VisitRecorder> logger, IConfiguration configuration)
    {
        // Try to obtain the path from configs
        var path = configuration["outputFile"];

        if (path is null)
        {
            // Linux host, /tmp/
            // Windows host, C:\Users\UserName\AppData\Local\Temp\
            path = Path.Combine(Path.GetTempPath(), "visits.log");
        }

        this.outputFilePath = path;
        this.logger = logger;
    }

    public async Task RecordVisit(Visit visit)
    {
        try
        {
            // Concurrency control
            await semaphore.WaitAsync();

            using (StreamWriter outputFile = new(outputFilePath, true))
            {
                var ipAddress = visit.IpAdress;
                var referrer = visit.Referrer ?? "null";
                var userAgent = visit.UserAgent ?? "null";
                var timeStamp = visit.UtcTimeStamp.ToString("o");

                var lineText = $"{timeStamp}|{referrer}|{userAgent}|{ipAddress}";
                await outputFile.WriteLineAsync(lineText);
            }

            logger.LogInformation($"{visit.Id} is completed");
            semaphore.Release();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error during Visit recording");
            throw;
        }
    }
}