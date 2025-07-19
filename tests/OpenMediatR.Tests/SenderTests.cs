using FluentAssertions;
using Moq;

namespace OpenMediatR.Tests;

public class SenderTests
{
    [Fact]
    public async Task Send_WhenNoPipelineConfigured_ShouldSendRequest()
    {
        // Arrange

        var handler = new TestHandler();
        var services = new Mock<IServiceProvider>();

        services.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, string>)))
            .Returns(handler);
        
        var sender = new OpenMediatRSender(services.Object);
        
        // Act
        var request = new TestRequest();
        var result = await sender.Send(request);
        
        // Assert
        result.Should().Be("Test");
    }
    
    [Fact]
    public async Task Send_WhenHandlerAndPipelineAreConfigured_ShouldExecutePipelineAndSendRequest()
    {
        // Arrange

        var handler = new TestHandler();
        var pipeline1 = new TestPipelineBehaviour1<TestRequest, string>();
        var pipeline2 = new TestPipelineBehaviour2<TestRequest, string>();
        var pipeline3 = new TestPipelineBehaviour1<TestRequest, string>();
        
        var services = new Mock<IServiceProvider>();

        services.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, string>)))
            .Returns(handler);
        
        var pipelineServiceType = typeof(IEnumerable<>).MakeGenericType(typeof(IPipelineBehaviour<TestRequest, string>));
        services.Setup(x => x.GetService(pipelineServiceType))
            .Returns(new List<IPipelineBehaviour<TestRequest, string>>() { pipeline1, pipeline2, pipeline3 });
        
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