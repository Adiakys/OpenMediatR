using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace OpenMediatR.Tests;

public class SenderTests
{
    [Fact]
    public async Task Send_WhenHandlerIsConfigured_ShouldSendRequest()
    {
        // Arrange
        var services = new Mock<IServiceProvider>();

        services.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, string>)))
            .Returns(new TestRequestHandler());
        
        var sender = new OpenMediatRSender(services.Object);
        
        // Act
        var request = new TestRequest();
        var result = await sender.Send(request);
        
        // Assert
        result.Should().Be("Test");
    }
}