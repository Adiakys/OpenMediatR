using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

internal sealed class MediatR : IMediatR
{
    private ISender? _sender;
    private IPublisher? _publisher;
    
    private readonly IServiceProvider _services;

    public MediatR(IServiceProvider services)
    {
        _services = services;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        _sender ??= _services.GetRequiredService<ISender>();
        return _sender.Send(request, cancellationToken);
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        _publisher ??= _services.GetRequiredService<IPublisher>();
        return _publisher.Publish(notification, cancellationToken);
    }
}