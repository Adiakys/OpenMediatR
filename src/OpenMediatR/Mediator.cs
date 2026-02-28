namespace OpenMediatR;

internal sealed class Mediator : IMediator
{
    private readonly ISender _sender;
    private readonly IPublisher _publisher;

    public Mediator(ISender sender, IPublisher publisher)
    {
        _sender = sender;
        _publisher = publisher;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return _sender.Send(request, cancellationToken);
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        return _publisher.Publish(notification, cancellationToken);
    }
}