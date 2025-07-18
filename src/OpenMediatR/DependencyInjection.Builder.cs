using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

public static partial class DependencyInjection
{
    private static void ConfigureService(this OpenMediatRBuilder builder, Type type, Dictionary<Type, Type> types)
    {
        if (type.IsAssignableTo(typeof(IRequest)))
        {
            builder.ConfigureRequest(type, types);
        }
            
        if (type.IsAssignableTo(typeof(IRequestHandler)))
        {
            builder.ConfigureHandler(type, types);
        }
    }

    private static void ConfigureRequest(this OpenMediatRBuilder builder, Type type, Dictionary<Type, Type> types)
    {
        foreach (var requestType in type.GetInterfaces()
                     .Where(x => x != typeof(IRequest) && x.IsAssignableTo(typeof(IRequest))))
        {
            var responseType = requestType.GetGenericArguments()[0];
            var handlerType = Type.MakeGenericSignatureType(typeof(IRequestHandler<,>), responseType);
                    
            if (types.TryGetValue(handlerType, out var handler))
            {
                builder.Services.AddTransient(type, handler);
                types.Remove(handlerType);
            }
            else
            {
                types.Add(requestType, type);
            }
        }
    }
   
    private static void ConfigureHandler(this OpenMediatRBuilder builder, Type type, Dictionary<Type, Type> types)
    {
        foreach (var handlerType in type.GetInterfaces()
                     .Where(x => x != typeof(IRequestHandler) && x.IsAssignableTo(typeof(IRequestHandler))))
        {
            var responseType = handlerType.GetGenericArguments()[1];
            var requestType = handlerType.GetGenericArguments()[0].GetInterfaces()
                .Where(x => x != typeof(IRequest) && x.IsAssignableTo(typeof(IRequest)))
                .First(x => x.GetGenericArguments()[0] == responseType);
            
            if (types.TryGetValue(requestType, out var request))
            {
                builder.Services.AddTransient(handlerType, type);
                types.Remove(requestType);
            }
            else
            {
                types.Add(handlerType, type);
            }
        }
    }
}