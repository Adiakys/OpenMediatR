namespace OpenMediatR.Tests;

internal class TestHandler : IRequestHandler<TestRequest, string>, INotificationHandler<TestNotification>
{
    public int RequestCount { get; private set; } = 0;
    public int NotificationCount { get; private set; } = 0;

    public virtual Task<string> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        RequestCount++;
        return Task.FromResult("Test");
    }

    public virtual Task Handle(TestNotification notification, CancellationToken cancellationToken)
    {
        NotificationCount++;
        return Task.CompletedTask;
    }
}