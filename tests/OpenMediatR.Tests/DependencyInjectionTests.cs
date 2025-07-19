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
            cfg.ConfigureServicesFromAssembly(typeof(DependencyInjectionTests).Assembly);
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
                x.ServiceType == typeof(IPipelineBehaviour) &&
                x.ImplementationType == typeof(TestPipelineBehaviour1))
            .Should().NotBeNull();
        
        services.FirstOrDefault(x =>
                x.ServiceType == typeof(IPipelineBehaviour) &&
                x.ImplementationType == typeof(TestPipelineBehaviour2))
            .Should().NotBeNull();
    }
}