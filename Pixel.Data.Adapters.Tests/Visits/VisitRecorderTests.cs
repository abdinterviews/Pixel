namespace Pixel.Data.Adapters.Visits.Tests;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pixel.Domain.Core.Visits.Entities;
using Xunit;

public class VisitRecorderTests
{
    [Fact]
    public async Task RecordVisit_WhenValidVisitPassed_ShouldWriteToOutputFile()
    {
        // Arrange
        var configuredFile = Path.GetTempFileName();
        var loggerMock = new Mock<ILogger<VisitRecorder>>();
        var configurationMock = new Mock<IConfiguration>();

        var visit = new Visit
        {
            Id = Ulid.NewUlid(),
            IpAdress = "192.168.1.1",
            Referrer = "https://example.com",
            UserAgent = "UserAgent",
            UtcTimeStamp = DateTime.UtcNow
        };

        configurationMock
            .Setup(config => config["outputFile"])
            .Returns(configuredFile);

        var target = new VisitRecorder(loggerMock.Object, configurationMock.Object);

        // Act
        await target.RecordVisit(visit);

        // Assert
        var lines = await File.ReadAllLinesAsync(configuredFile);
        Assert.True(lines.Length == 1);
        Assert.Equal($"{visit.UtcTimeStamp.ToString("o")}|{visit.Referrer}|{visit.UserAgent}|{visit.IpAdress}", lines.First());
        File.Delete(configuredFile);
    }

    [Fact]
    public async Task RecordVisit_WhenValidVisitPassed_ShouldWriteToOutputFile_WithoutReferrer()
    {
        // Arrange
        var configuredFile = Path.GetTempFileName();
        var loggerMock = new Mock<ILogger<VisitRecorder>>();
        var configurationMock = new Mock<IConfiguration>();

        var visit = new Visit
        {
            Id = Ulid.NewUlid(),
            IpAdress = "192.168.1.1",
            Referrer = null,
            UserAgent = "UserAgent",
            UtcTimeStamp = DateTime.UtcNow
        };

        configurationMock
            .Setup(config => config["outputFile"])
            .Returns(configuredFile);

        var target = new VisitRecorder(loggerMock.Object, configurationMock.Object);

        // Act
        await target.RecordVisit(visit);

        // Assert
        var lines = await File.ReadAllLinesAsync(configuredFile);
        Assert.True(lines.Length == 1);
        Assert.Equal($"{visit.UtcTimeStamp.ToString("o")}|null|{visit.UserAgent}|{visit.IpAdress}", lines.First());
        File.Delete(configuredFile);
    }

    [Fact]
    public async Task RecordVisit_WhenValidVisitPassed_ShouldWriteToOutputFile_WithoutUserAgent()
    {
        // Arrange
        var configuredFile = Path.GetTempFileName();
        var loggerMock = new Mock<ILogger<VisitRecorder>>();
        var configurationMock = new Mock<IConfiguration>();

        var visit = new Visit
        {
            Id = Ulid.NewUlid(),
            IpAdress = "192.168.1.1",
            Referrer = "https://example.com",
            UserAgent = null,
            UtcTimeStamp = DateTime.UtcNow
        };

        configurationMock
            .Setup(config => config["outputFile"])
            .Returns(configuredFile);

        var target = new VisitRecorder(loggerMock.Object, configurationMock.Object);

        // Act
        await target.RecordVisit(visit);

        // Assert
        var lines = await File.ReadAllLinesAsync(configuredFile);
        Assert.True(lines.Length == 1);
        Assert.Equal($"{visit.UtcTimeStamp.ToString("o")}|{visit.Referrer}|null|{visit.IpAdress}", lines.First());
        File.Delete(configuredFile);
    }

    [Fact]
    public async Task RecordVisit_WhenConfigurationIsNull_ShouldUseDefaultOutputFile()
    {
        // Arrange
        var defaultFile = Path.Combine(Path.GetTempPath(), "visits.log"); ;
        var loggerMock = new Mock<ILogger<VisitRecorder>>();
        var configurationMock = new Mock<IConfiguration>();

        var visit = new Visit
        {
            Id = Ulid.NewUlid(),
            IpAdress = "192.168.1.1",
            Referrer = "https://example.com",
            UserAgent = "UserAgent",
            UtcTimeStamp = DateTime.UtcNow
        };

        var target = new VisitRecorder(loggerMock.Object, configurationMock.Object);

        // Act
        await target.RecordVisit(visit);

        // Assert
        var lines = await File.ReadAllLinesAsync(defaultFile);
        Assert.True(lines.Length == 1);
        Assert.Equal($"{visit.UtcTimeStamp.ToString("o")}|{visit.Referrer}|{visit.UserAgent}|{visit.IpAdress}", lines.First());
        File.Delete(defaultFile);
    }
}