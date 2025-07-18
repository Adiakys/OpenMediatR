using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

internal sealed class OpenMediatRSender : ISender
{
    private readonly IServiceProvider _services;

    public OpenMediatRSender(IServiceProvider services)
    {
        _services = services;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _services.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("Handle", [request.GetType(), typeof(CancellationToken)]);
        return (handleMethod?.Invoke(handler, [request, cancellationToken]) as Task<TResponse>)!;
    }
}