using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR;

public static partial class DependencyInjection
{
    private static readonly Dictionary<Type, Type> OrphanTypes = [];
    private static bool FirstConfig { get; set; } = false;
    
    public static IServiceCollection AddOpenMediatR(this IServiceCollection services, Action<OpenMediatRBuilder> builder)
    {
        if (FirstConfig is false)
        {
            services.AddTransient<ISender, OpenMediatRSender>();
            FirstConfig = true;
        }
        
        builder(new OpenMediatRBuilder(services));
        
        return services;
    }
    
    public static OpenMediatRBuilder ConfigureServicesFromAssemblies(this OpenMediatRBuilder builder, IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            builder.ConfigureServicesFromAssembly(assembly);
        }
        
        return builder;
    }

    public static OpenMediatRBuilder ConfigureServicesFromAssembly(this OpenMediatRBuilder builder, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsInterface || type.IsAbstract || type.IsClass is false)
            {
                continue;
            }
            
            builder.ConfigureService(type, OrphanTypes);
        }
        
        return builder;
    }

    public sealed class OpenMediatRBuilder
    {
        internal IServiceCollection Services { get; private init; }

        internal OpenMediatRBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}