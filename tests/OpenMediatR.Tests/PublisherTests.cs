using Moq;

namespace OpenMediatR.Tests;

public class PublisherTests
{
    [Fact]
    public async Task Should_be_able_to_publish_notifications()
    {
        var sink1 = new Mock<INotificationSink>();
        sink1.Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var sink2 = new Mock<INotificationSink>();
        sink2.Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var services = new Mock<IServiceProvider>();
        services.Setup(x => x.GetService(typeof(IEnumerable<INotificationSink>)))
            .Returns((IEnumerable<INotificationSink>)[sink1.Object, sink2.Object]);
        
        var publisher = new OpenMediatRPublisher(services.Object);
        
        await publisher.Publish(new TestNotification());
        
        sink1.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        sink2.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}