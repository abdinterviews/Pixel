namespace Pixel.Storage.Service.Tests.Visits.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Pixel.Domain.Core.Visits.Entities;
using Pixel.Domain.Core.Visits.Ports;
using Pixel.Messages.Visits.Commands;
using Pixel.Storage.Service.Visits.Consumers;
using System.Threading.Tasks;

public class RecordVisitCommandConsumerTests
{
    [Fact]
    public async Task RecordVisitCommandConsumer_ShouldCallRecorder()
    {
        // Arrange
        var mockIVisitRecorder = new Mock<IVisitRecorder>();

        var target = new RecordVisitCommandConsumer(new NullLogger<RecordVisitCommandConsumer>(), mockIVisitRecorder.Object);

        var mockConsumeContext = new Mock<ConsumeContext<RecordVisitCommand>>();

        var message = new RecordVisitCommand
        {
            IpAdress = "123",
            Referrer = "456",
            UserAgent = "789",
        };

        mockConsumeContext
            .SetupGet(m => m.Message)
            .Returns(message);

        Visit? recordedVisit = null;
        mockIVisitRecorder
            .Setup(m => m.RecordVisit(It.IsAny<Visit>()))
            .Callback<Visit>(c => recordedVisit = c);

        // Act
        await target.Consume(mockConsumeContext.Object);

        // Assert
        mockIVisitRecorder.Verify(m => m.RecordVisit(It.IsAny<Visit>()), Times.Once);

        Assert.NotNull(recordedVisit);
        Assert.Equal(message.MessageId, recordedVisit.Id);
        Assert.Equal(message.IpAdress, recordedVisit.IpAdress);
        Assert.Equal(message.Referrer, recordedVisit.Referrer);
        Assert.Equal(message.UserAgent, recordedVisit.UserAgent);
        Assert.Equal(message.UtcTimeStamp, recordedVisit.UtcTimeStamp);
    }
}