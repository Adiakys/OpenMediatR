using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

public static partial class DependencyInjection
{
    private static void ConfigureService(this OpenMediatRBuilder builder, Type type)
    {
        if (type.IsAssignableTo(typeof(IRequestHandler)))
        {
            builder.ConfigureHandler<IRequestHandler>(type);
        }
            
        if (type.IsAssignableTo(typeof(INotificationHandler)))
        {
            builder.ConfigureHandler<INotificationHandler>(type);
        }
    }
    
    private static void ConfigureHandler<T>(this OpenMediatRBuilder builder, Type type)
    {
        foreach (var handlerType in type.GetInterfaces()
                     .Where(x => x != typeof(T) && x.IsAssignableTo(typeof(T))))
        {
            builder.Services.AddTransient(handlerType, type);
        }
    }
}