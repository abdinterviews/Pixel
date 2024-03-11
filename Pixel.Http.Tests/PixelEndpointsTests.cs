namespace Pixel.Http.Tests;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Moq;
using Pixel.Messages.Visits.Commands;

public class PixelEndpointsTests
{
    [Fact]
    public async Task PixelEndpoints_Track_ShouldReturnBadRequestIfIpIsMissing()
    {
        // Arrange
        var mockILoggerFactory = new Mock<ILoggerFactory>();
        var mockIPublishEndpoint = new Mock<IPublishEndpoint>();
        var mockIMemoryCache = new Mock<IMemoryCache>();
        var mockHttpRequest = new Mock<HttpRequest>();

        var mockHttpContext = new Mock<HttpContext>();
        var mockConnection = new Mock<ConnectionInfo>();

        mockHttpRequest
            .SetupGet(p => p.HttpContext)
            .Returns(mockHttpContext.Object);

        mockHttpContext
            .SetupGet(p => p.Connection)
            .Returns(mockConnection.Object);

        mockConnection
            .SetupGet(p => p.RemoteIpAddress)
            .Returns(null as IPAddress);

        // Act
        var result = await PixelEndpoints.Track(
            new NullLoggerFactory(),
            mockIPublishEndpoint.Object,
            mockIMemoryCache.Object,
            mockHttpRequest.Object,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<BadRequest>(result);
    }

    [Fact]
    public async Task PixelEndpoints_Track_ShouldSendMessage()
    {
        // Arrange
        var expectedIpAddress = "127.0.0.1";
        var expectedReferrer = "Referrer";
        var expectedUserAgent = "user-agent";

        var mockILoggerFactory = new Mock<ILoggerFactory>();
        var mockIPublishEndpoint = new Mock<IPublishEndpoint>();

        var memoryCache = Mock.Of<IMemoryCache>();
        var mockIMemoryCache = Mock.Get(memoryCache);

        var mockHttpRequest = new Mock<HttpRequest>();

        var mockHttpContext = new Mock<HttpContext>();
        var mockConnection = new Mock<ConnectionInfo>();

        var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "Referrer", expectedReferrer},
                { "User-Agent", expectedUserAgent},
            }) as IHeaderDictionary;

        mockHttpRequest
            .SetupGet(p => p.Headers)
            .Returns(headers);

        mockHttpRequest
            .SetupGet(p => p.HttpContext)
            .Returns(mockHttpContext.Object);

        mockHttpContext
            .SetupGet(p => p.Connection)
            .Returns(mockConnection.Object);

        mockConnection
            .SetupGet(p => p.RemoteIpAddress)
            .Returns(IPAddress.Parse(expectedIpAddress));

        var cachEntry = Mock.Of<ICacheEntry>();
        mockIMemoryCache
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cachEntry);

        RecordVisitCommand? command = null;
        mockIPublishEndpoint
            .Setup(m => m.Publish(It.IsAny<RecordVisitCommand>(), It.IsAny<CancellationToken>()))
            .Callback<RecordVisitCommand, CancellationToken>((m, c) =>
            {
                command = m;
            });

        // Act
        var result = await PixelEndpoints.Track(new NullLoggerFactory(), mockIPublishEndpoint.Object, mockIMemoryCache.Object, mockHttpRequest.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IFileHttpResult>(result);

        Assert.NotNull(command);
        Assert.Equal(expectedReferrer, command.Referrer);
        Assert.Equal(expectedUserAgent, command.UserAgent);
        Assert.Equal(expectedIpAddress, command.IpAdress);
        Assert.NotEqual(DateTime.MinValue, command.UtcTimeStamp);
    }
}