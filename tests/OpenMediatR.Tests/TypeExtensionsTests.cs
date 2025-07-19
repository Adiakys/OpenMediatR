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
        typeof(TestPipelineBehaviour1<,>).ImplementsMediatR(DependencyInjection.PipelineBehaviourType).Should().BeTrue();
        
        typeof(TestPipelineBehaviour1<,>).ImplementsMediatR(DependencyInjection.RequestHandlerType).Should().BeFalse();
        typeof(TestRequest).ImplementsMediatR(DependencyInjection.PipelineBehaviourType).Should().BeFalse();
        typeof(string).ImplementsMediatR(DependencyInjection.PipelineBehaviourType).Should().BeFalse();
    }

    [Fact]
    public void GetServicesOrDefault_WhenNoServiceConfigured_ShouldReturnEmptyList()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var serviceType = typeof(IPipelineBehaviour<,>).MakeGenericType(typeof(TestRequest), typeof(string));
        var result = serviceProvider.GetServicesOrDefault(serviceType);
        
        result.Should().BeEmpty();
    }
    
    [Fact]
    public void GetServicesOrDefault_When2ServiceIsConfigured_ShouldReturnServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddTransient(typeof(IPipelineBehaviour<,>), typeof(TestPipelineBehaviour1<,>));
        serviceCollection.AddTransient(typeof(IPipelineBehaviour<,>), typeof(TestPipelineBehaviour2<,>));
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var serviceType = typeof(IPipelineBehaviour<,>).MakeGenericType(typeof(TestRequest), typeof(string));
        var result = serviceProvider.GetServicesOrDefault(serviceType);
        
        result.Should().HaveCount(2);
    }
}