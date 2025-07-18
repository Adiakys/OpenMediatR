using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

internal sealed class OpenMediatRPublisher : IPublisher
{
    private readonly IServiceProvider _services;

    public OpenMediatRPublisher(IServiceProvider services)
    {
        _services = services;
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        var sinks = _services.GetServices<INotificationSink>();

        foreach (var sink in sinks)
        {
            await sink.Publish(notification, cancellationToken);
        }
    }
}