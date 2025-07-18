using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

internal sealed class MediatR : IMediatR
{
    private ISender? Sender;
    private IPublisher Publisher;
    
    private readonly IServiceProvider _services;

    public MediatR(IServiceProvider services)
    {
        _services = services;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        Sender ??= _services.GetRequiredService<ISender>();
        return Sender.Send(request, cancellationToken);
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        Publisher ??= _services.GetRequiredService<IPublisher>();
        return Publisher.Publish(notification, cancellationToken);
    }
}