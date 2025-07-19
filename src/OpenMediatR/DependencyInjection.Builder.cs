using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

public static partial class DependencyInjection
{
    private static void ConfigureService(this OpenMediatRBuilder builder, Type type)
    {
        if (builder.AutoRegisterRequestHandlers && type.IsAssignableTo(typeof(IRequestHandler)))
        {
            builder.Configure<IRequestHandler>(type);
        }
            
        if (builder.AutoRegisterNotificationHandlers && type.IsAssignableTo(typeof(INotificationHandler)))
        {
            builder.Configure<INotificationHandler>(type);
        }
        
        if (builder.AutoRegisterPipelineBehaviours && type.ImplementsInterface<IPipelineBehaviour>())
        {
            builder.Services.AddTransient(typeof(IPipelineBehaviour), type);
        }
    }
    
    private static void Configure<T>(this OpenMediatRBuilder builder, Type type)
    {
        foreach (var handlerType in type.GetInterfaces().Where(x => x.Implements<T>()))
        {
            builder.Services.AddTransient(handlerType, type);
        }
    }
    
    internal static bool ImplementsInterface<T>(this Type type, Type requestType)
        => type.GetInterfaces().Any(x => x.Implements<T>() && requestType.IsAssignableTo(x.GetGenericArguments()[0]));
    
    private static bool ImplementsInterface<T>(this Type type)
        => type.GetInterfaces().Any(x => x.Implements<T>());
    
    private static bool Implements<T>(this Type type)
        => type != typeof(T) && type.IsAssignableTo(typeof(T));
}