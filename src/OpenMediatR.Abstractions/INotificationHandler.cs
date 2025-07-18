namespace OpenMediatR;

public interface INotificationHandler
{
}

public interface INotificationHandler<in TNotification> : INotificationHandler
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}