using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMediatR.Tests;

public sealed class TypeExtensionsTests
{
    [Fact]
    public void ImplementsMediatR()
    {
        typeof(TestHandler).ImplementsMediatR(DependencyInjection.RequestHandlerType).Should().BeTrue();
        typeof(TestHandler).ImplementsMediatR(DependencyInjection.NotificationHandlerType).Should().BeTrue();
        typeof(TestPipelineBehavior1<,>).ImplementsMediatR(DependencyInjection.PipelineBehaviorType).Should().BeTrue();
        
        typeof(TestPipelineBehavior1<,>).ImplementsMediatR(DependencyInjection.RequestHandlerType).Should().BeFalse();
        typeof(TestRequest).ImplementsMediatR(DependencyInjection.PipelineBehaviorType).Should().BeFalse();
        typeof(string).ImplementsMediatR(DependencyInjection.PipelineBehaviorType).Should().BeFalse();
    }

    [Fact]
    public void GetServicesOrDefault_WhenNoServiceConfigured_ShouldReturnEmptyList()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var serviceType = typeof(IPipelineBehavior<,>).MakeGenericType(typeof(TestRequest), typeof(string));
        var result = serviceProvider.GetServicesOrDefault(serviceType);
        
        result.Should().BeEmpty();
    }
    
    [Fact]
    public void GetServicesOrDefault_When2ServiceIsConfigured_ShouldReturnServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(TestPipelineBehavior1<,>));
        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(TestPipelineBehavior2<,>));
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var serviceType = typeof(IPipelineBehavior<,>).MakeGenericType(typeof(TestRequest), typeof(string));
        var result = serviceProvider.GetServicesOrDefault(serviceType);
        
        result.Should().HaveCount(2);
    }
}