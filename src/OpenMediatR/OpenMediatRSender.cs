using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

internal sealed class OpenMediatRSender : ISender
{
    private static readonly ConcurrentDictionary<Type, RequestMetadata> MetadataCache = new();

    private readonly IServiceProvider _services;

    public OpenMediatRSender(IServiceProvider services)
    {
        _services = services;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var metadata = GetOrCreateMetadata(request.GetType());
        RequestHandlerDelegate<TResponse> next = () => Handle(request, metadata, cancellationToken);
        next = ConfigurePipeline(request, metadata, next, cancellationToken);
        return next();
    }

    private Task<TResponse> Handle<TResponse>(IRequest<TResponse> request, RequestMetadata metadata, CancellationToken cancellationToken)
    {
        try
        {
            var handler = _services.GetRequiredService(metadata.HandlerType);
            return (metadata.HandlerMethod.Invoke(handler, [request, cancellationToken]) as Task<TResponse>)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw; // unreachable
        }
    }

    private RequestHandlerDelegate<TResponse> ConfigurePipeline<TResponse>(
        IRequest<TResponse> request,
        RequestMetadata metadata,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var pipelines = _services.GetServicesOrDefault(metadata.PipelineType);

        foreach (var pipeline in pipelines)
        {
            var currentNext = next;
            next = () =>
            {
                try
                {
                    var result = metadata.PipelineMethod.Invoke(pipeline, [request, currentNext, cancellationToken]);
                    return (Task<TResponse>)result!;
                }
                catch (TargetInvocationException ex) when (ex.InnerException is not null)
                {
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                    throw; // unreachable
                }
            };
        }

        return next;
    }

    private static RequestMetadata GetOrCreateMetadata(Type requestType)
    {
        return MetadataCache.GetOrAdd(requestType, _ =>
        {
            var responseType = requestType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];

            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            var handlerMethod = handlerType.GetMethod("Handle", [requestType, typeof(CancellationToken)])!;

            var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
            var pipelineMethod = pipelineType.GetMethod("Handle");

            return new RequestMetadata(handlerType, handlerMethod, pipelineType, pipelineMethod!);
        });
    }

    private sealed record RequestMetadata(
        Type HandlerType,
        MethodInfo HandlerMethod,
        Type PipelineType,
        MethodInfo PipelineMethod);
}