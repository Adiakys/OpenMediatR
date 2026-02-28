using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void ConfigureServiceFromAssembly_WhenAServiceMatch_ShouldAddService()
    {
        var services = new ServiceCollection();

        services.AddOpenMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<DependencyInjectionTests>();
            cfg.AddOpenBehavior(typeof(TestPipelineBehavior1<,>));
            cfg.AddOpenBehavior(typeof(TestPipelineBehavior2<,>));
        });

        services.FirstOrDefault(x =>
                x.ServiceType == typeof(IRequestHandler<TestRequest, string>) &&
                x.ImplementationType == typeof(TestHandler))
            .Should().NotBeNull();
        
        services.FirstOrDefault(x =>
                x.ServiceType == typeof(IRequestHandler<TestRequest2, bool>) &&
                x.ImplementationType == typeof(TestHandler))
            .Should().NotBeNull();
        
        services.FirstOrDefault(x =>
                x.ServiceType == typeof(INotificationHandler<TestNotification>) &&
                x.ImplementationType == typeof(TestHandler))
            .Should().NotBeNull();
        
        services.FirstOrDefault(x =>
                x.ServiceType == typeof(IPipelineBehavior<,>) &&
                x.ImplementationType == typeof(TestPipelineBehavior1<,>))
            .Should().NotBeNull();
        
        services.FirstOrDefault(x =>
                x.ServiceType == typeof(IPipelineBehavior<,>) &&
                x.ImplementationType == typeof(TestPipelineBehavior2<,>))
            .Should().NotBeNull();
    }
}