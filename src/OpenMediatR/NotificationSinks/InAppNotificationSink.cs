using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR.NotificationSinks;

internal sealed class InAppNotificationSink : INotificationSink
{
    private readonly IServiceProvider _services;

    public InAppNotificationSink(IServiceProvider services)
    {
        _services = services;
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = _services.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod("Handle", [notification.GetType(), typeof(CancellationToken)]);
            await (handleMethod?.Invoke(handler, [notification, cancellationToken]) as Task)!;
        }
    }
}