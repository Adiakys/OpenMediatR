using FluentAssertions;
using Moq;
using OpenMediatR.NotificationPublishers;
using OpenMediatR.NotificationSinks;

namespace OpenMediatR.Tests;

public sealed class InMemoryNotificationSinkTests
{
    [Fact]
    public async Task Dispatch_WithMultipleHandlers_ShouldExecuteAll()
    {
        var handler1 = new TestHandler();
        var handler2 = new TestHandler();

        var services = new Mock<IServiceProvider>();
        services.Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<TestNotification>>)))
            .Returns(new List<INotificationHandler<TestNotification>> { handler1, handler2 });

        var sink = new InMemoryNotificationSink(services.Object, new ForeachAwaitPublisher());

        await sink.Dispatch(new TestNotification());

        handler1.NotificationCount.Should().Be(1);
        handler2.NotificationCount.Should().Be(1);
    }

    [Fact]
    public async Task Dispatch_WhenHandlerThrows_ShouldPropagateOriginalException()
    {
        var expectedException = new InvalidOperationException("handler failed");
        var handler = new Mock<INotificationHandler<TestNotification>>();
        handler.Setup(x => x.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var services = new Mock<IServiceProvider>();
        services.Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<TestNotification>>)))
            .Returns(new List<INotificationHandler<TestNotification>> { handler.Object });

        var sink = new InMemoryNotificationSink(services.Object, new ForeachAwaitPublisher());

        var act = () => sink.Dispatch(new TestNotification());

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Should().BeSameAs(expectedException);
    }

    [Fact]
    public async Task Dispatch_WithNoHandlers_ShouldNotThrow()
    {
        var services = new Mock<IServiceProvider>();
        services.Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<TestNotification>>)))
            .Returns(new List<INotificationHandler<TestNotification>>());

        var sink = new InMemoryNotificationSink(services.Object, new ForeachAwaitPublisher());

        var act = () => sink.Dispatch(new TestNotification());

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Dispatch_WithTaskWhenAllPublisher_ShouldExecuteAllHandlers()
    {
        var handler1 = new TestHandler();
        var handler2 = new TestHandler();

        var services = new Mock<IServiceProvider>();
        services.Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<TestNotification>>)))
            .Returns(new List<INotificationHandler<TestNotification>> { handler1, handler2 });

        var sink = new InMemoryNotificationSink(services.Object, new TaskWhenAllPublisher());

        await sink.Dispatch(new TestNotification());

        handler1.NotificationCount.Should().Be(1);
        handler2.NotificationCount.Should().Be(1);
    }
}
