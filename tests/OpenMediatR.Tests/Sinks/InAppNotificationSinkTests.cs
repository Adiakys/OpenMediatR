using FluentAssertions;
using Moq;
using OpenMediatR.NotificationSinks;

namespace OpenMediatR.Tests.Sinks;

public class InAppNotificationSinkTests
{
    [Fact]
    public async Task Should_be_able_to_send_notifications()
    {
        var handler = new TestHandler();
        var services = new Mock<IServiceProvider>();
        services.Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<TestNotification>>)))
            .Returns(new List<INotificationHandler<TestNotification>>() { handler });

        var sink = new InAppNotificationSink(services.Object);
        
        await sink.Publish(new TestNotification());
        
        handler.RequestCount.Should().Be(0);
        handler.NotificationCount.Should().Be(1);
    }
}