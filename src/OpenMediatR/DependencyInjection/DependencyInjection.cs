using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenMediatR.NotificationSinks;

namespace OpenMediatR;

public static partial class DependencyInjection
{
    private static bool FirstConfig { get; set; } = false;
    
    public static IServiceCollection AddOpenMediatR(this IServiceCollection services, Action<OpenMediatRServiceConfiguration> builder)
    {
        if (FirstConfig is false)
        {
            
            services.AddTransient<ISender, OpenMediatRSender>();
            
            services.AddTransient<IPublisher, OpenMediatRPublisher>();
            services.AddTransient<INotificationSink, InAppNotificationSink>();
            
            services.AddTransient<IMediatR, MediatR>();
            
            FirstConfig = true;
        }
        
        builder(new OpenMediatRServiceConfiguration(services));
        
        return services;
    }
    
    public static OpenMediatRServiceConfiguration ConfigureServicesFromAssemblies(this OpenMediatRServiceConfiguration serviceConfiguration, IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            serviceConfiguration.ConfigureServicesFromAssembly(assembly);
        }
        
        return serviceConfiguration;
    }

    public static OpenMediatRServiceConfiguration ConfigureServicesFromAssembly(this OpenMediatRServiceConfiguration serviceConfiguration, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsInterface || type.IsAbstract || type.IsClass is false)
            {
                continue;
            }
            
            serviceConfiguration.ConfigureService(type);
        }
        
        return serviceConfiguration;
    }
    
    public sealed class OpenMediatRServiceConfiguration
    {
        public bool AutoRegisterRequestHandlers { get; set; } = true;
        
        public bool AutoRegisterNotificationHandlers { get; set; } = true;
        
        public bool AutoRegisterPipelineBehaviours { get; set; } = true;
        
        internal IServiceCollection Services { get; private init; }

        internal OpenMediatRServiceConfiguration(IServiceCollection services)
        {
            Services = services;
        }
    }
}