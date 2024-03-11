namespace Pixel.Http;

using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Pixel.Messages.Visits.Commands;

public static class PixelEndpoints
{
    public static RouteGroupBuilder MapPixelApi(this RouteGroupBuilder group)
    {
        group.MapGet("/track", Track);
        return group;
    }

    public static async Task<IResult> Track(
            ILoggerFactory loggerFactory,
            IPublishEndpoint publishEndpoint,
            IMemoryCache memoryCache,
            HttpRequest request,
            CancellationToken cancellation)
    {
        var logger = loggerFactory.CreateLogger(typeof(PixelEndpoints));

        // Extract information
        var ip = request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();

        // Ip information is a required field
        if (ip is null)
        {
            logger.LogWarning("Could not retrieve IP from request");
            return Results.BadRequest();
        }

        const string ReferrerHeaderKey = "Referrer";
        request.Headers.TryGetValue(ReferrerHeaderKey, out var Referrer);

        const string userAgentHeaderKey = "User-Agent";
        request.Headers.TryGetValue(userAgentHeaderKey, out var userAgent);

        logger.LogInformation("{TimeStamp}|{Referrer}|{UserAgent}|{Ip}",
            DateTime.UtcNow.ToString("o"),
            Referrer,
            userAgent,
            ip);

        // Create and publish record command
        var command = new RecordVisitCommand
        {
            IpAdress = ip,
            Referrer = Referrer,
            UserAgent = userAgent,
        };
        await publishEndpoint.Publish(command, cancellation);

        // Try to load the result from the memory cache to not load from disk
        const string fileKey = "fileKey";
        if (!memoryCache.TryGetValue(fileKey, out IResult? cachedResult))
        {
            // Load file from source
            const string fileName = "track.gif";
            const string assetsFolder = "Assets";
            const string resultType = "image/gif";

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), assetsFolder, fileName);
            var bytes = await File.ReadAllBytesAsync(filePath, cancellation);

            cachedResult = Results.File(bytes, resultType, fileName);

            memoryCache.Set(fileKey, cachedResult, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1)
            });
        }

        return cachedResult!;
    }
}