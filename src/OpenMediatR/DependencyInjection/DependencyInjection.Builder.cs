using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

public static partial class DependencyInjection
{
    internal static readonly Type RequestHandlerType = typeof(IRequestHandler<,>);
    internal static readonly Type NotificationHandlerType = typeof(INotificationHandler<>);
    internal static readonly Type PipelineBehaviourType = typeof(IPipelineBehaviour<,>);
    
    private static void ConfigureService(this OpenMediatRServiceConfiguration serviceConfiguration, Type type)
    {
        foreach (var interfaceType in type.GetInterfaces())
        {
            if (serviceConfiguration.AutoRegisterRequestHandlers && type.ImplementsMediatR(RequestHandlerType) ||
                serviceConfiguration.AutoRegisterNotificationHandlers && type.ImplementsMediatR(NotificationHandlerType))
            {
                serviceConfiguration.Services.AddTransient(interfaceType, type);
                continue;
            }

            if (serviceConfiguration.AutoRegisterPipelineBehaviours && type.ImplementsMediatR(PipelineBehaviourType))
            {
                serviceConfiguration.Services.AddTransient(PipelineBehaviourType, type);
                continue;
            }
        }
    }
}