using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

internal static class TypeExtensions
{
    internal static IEnumerable<object> GetServicesOrDefault(this IServiceProvider services, Type serviceType)
    {
        var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(serviceType);
        return (IEnumerable<object>?)services.GetService(genericEnumerable) ?? [];
    }
    
    internal static bool ImplementsMediatR(this Type type, Type baseType)
        => type.GetInterfaces()
            .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == baseType);
}