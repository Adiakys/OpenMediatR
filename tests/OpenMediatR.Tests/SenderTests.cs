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

        var handler = new TestHandler();
        var pipeline1 = new TestPipelineBehaviour1();
        var pipeline2 = new TestPipelineBehaviour2();
        var pipeline3 = new TestPipelineBehaviour1();
        
        var services = new Mock<IServiceProvider>();

        services.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, string>)))
            .Returns(handler);
        
        services.Setup(x => x.GetService(typeof(IEnumerable<IPipelineBehaviour>)))
            .Returns(new List<IPipelineBehaviour>() { pipeline1, pipeline2, pipeline3 });
        
        var sender = new OpenMediatRSender(services.Object);
        
        // Act
        var request = new TestRequest();
        var result = await sender.Send(request);
        
        // Assert
        result.Should().Be("Test");
        pipeline1.Count.Should().Be(1);
        pipeline2.Count.Should().Be(1);
        pipeline3.Count.Should().Be(1);
    }
}