namespace OpenMediatR;

/// <summary>
/// Defines the strategy for executing notification handlers.
/// </summary>
public interface INotificationPublisher
{
    Task Publish(IEnumerable<Func<Task>> handlerCalls, CancellationToken cancellationToken);
}
