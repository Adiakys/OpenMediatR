namespace OpenMediatR;

/// <summary>
/// Entry point for publishing notifications.
/// Fans out to all registered <see cref="INotificationSink"/> implementations sequentially.
/// All sinks are executed even if one throws; exceptions are logged and do not propagate.
/// </summary>
public interface IPublisher
{
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}