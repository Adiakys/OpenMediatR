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
        RequestHandlerDelegate<TResponse> next = (t) => this.Handle(request, t);
        next = this.ConfigurePipeline(request, next);
        return next(cancellationToken);
    }

    private Task<TResponse> Handle<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _services.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("Handle", [request.GetType(), typeof(CancellationToken)]);
        return (handleMethod?.Invoke(handler, [request, cancellationToken]) as Task<TResponse>)!;
    }

    private RequestHandlerDelegate<TResponse> ConfigurePipeline<TResponse>(IRequest<TResponse> request, RequestHandlerDelegate<TResponse> next)
    {
        var pipelineType = typeof(IPipelineBehaviour<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handleMethod = pipelineType.GetMethod("Handle", [request.GetType(), next.GetType(), typeof(CancellationToken)]);

        if (handleMethod is null)
        {
            return next;
        }
        
        var pipelines = _services.GetService<IEnumerable<IPipelineBehaviour>>() ?? [];
        
        foreach (var pipeline in pipelines)
        {
            if (pipeline.GetType().ImplementsInterface<IPipelineBehaviour>(request.GetType()) is false)
            {
                continue;
            }
            
            var currentNext = next;
            next = (ct) =>
            {
                var result = handleMethod.Invoke(pipeline, [request, currentNext, ct]);
                return (Task<TResponse>)result!;
            };
        }

        return next;
    }
}