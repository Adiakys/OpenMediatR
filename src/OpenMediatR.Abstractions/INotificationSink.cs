namespace OpenMediatR;

/// <summary>
/// Represents a delivery channel for notifications.
/// Each sink implementation determines how notifications are dispatched
/// (e.g., in-memory handlers, HTTP webhooks, message brokers).
/// </summary>
public interface INotificationSink
{
    Task Dispatch<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}