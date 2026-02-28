using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

public static partial class DependencyInjection
{
    internal static readonly Type RequestHandlerType = typeof(IRequestHandler<,>);
    internal static readonly Type NotificationHandlerType = typeof(INotificationHandler<>);
    internal static readonly Type PipelineBehaviorType = typeof(IPipelineBehavior<,>);
    
    private static void ConfigureService(this OpenMediatRServiceConfiguration serviceConfiguration, Type type)
    {
        foreach (var interfaceType in type.GetInterfaces())
        {
            if (!interfaceType.IsGenericType)
                continue;

            var genericDefinition = interfaceType.GetGenericTypeDefinition();

            if (serviceConfiguration.AutoRegisterRequestHandlers && genericDefinition == RequestHandlerType ||
                serviceConfiguration.AutoRegisterNotificationHandlers && genericDefinition == NotificationHandlerType)
            {
                serviceConfiguration.Services.AddTransient(interfaceType, type);
                continue;
            }

            if (serviceConfiguration.AutoRegisterPipelineBehaviors && genericDefinition == PipelineBehaviorType)
            {
                serviceConfiguration.Services.AddTransient(PipelineBehaviorType, type);
                continue;
            }
        }
    }
}