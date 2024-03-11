namespace Pixel.Storage.Service.Visits.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using Pixel.Domain.Core.Visits.Entities;
using Pixel.Domain.Core.Visits.Ports;
using Pixel.Messages.Visits.Commands;

public class RecordVisitCommandConsumer(ILogger<RecordVisitCommandConsumer> logger, IVisitRecorder recorder) : IConsumer<RecordVisitCommand>
{
    private readonly IVisitRecorder recorder = recorder;
    private readonly ILogger<RecordVisitCommandConsumer> logger = logger;

    public async Task Consume(ConsumeContext<RecordVisitCommand> context)
    {
        try
        {
            logger.LogInformation($"Received recording command for {context.Message.UtcTimeStamp.ToString("o")}|{context.Message.Referrer}|{context.Message.UserAgent}|{context.Message.IpAdress}");

            var visit = new Visit
            {
                Id = context.Message.MessageId,
                IpAdress = context.Message.IpAdress,
                Referrer = context.Message.Referrer,
                UserAgent = context.Message.UserAgent,
                UtcTimeStamp = context.Message.UtcTimeStamp,
            };

            await this.recorder.RecordVisit(visit);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failure during RecordVisitCommand consumer handling");
            throw;
        }
    }
}
