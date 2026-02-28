using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMediatR.NotificationSinks;

namespace OpenMediatR.Tests;

public sealed class DependencyInjectionTests
{
    private static IServiceCollection CreateServices(Action<OpenMediatRConfiguration>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddOpenMediatR(configure ?? (cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<DependencyInjectionTests>();
            cfg.AddOpenBehavior(typeof(TestPipelineBehavior1<,>));
            cfg.AddOpenBehavior(typeof(TestPipelineBehavior2<,>));
        }));
        return services;
    }

    [Fact]
    public void AddOpenMediatR_ShouldRegisterRequestHandlersFromAssembly()
    {
        var services = CreateServices();

        services.Should().Contain(x =>
            x.ServiceType == typeof(IRequestHandler<TestRequest, string>) &&
            x.ImplementationType == typeof(TestHandler));

        services.Should().Contain(x =>
            x.ServiceType == typeof(IRequestHandler<TestRequest2, bool>) &&
            x.ImplementationType == typeof(TestHandler));
    }

    [Fact]
    public void AddOpenMediatR_ShouldRegisterNotificationHandlersFromAssembly()
    {
        var services = CreateServices();

        services.Should().Contain(x =>
            x.ServiceType == typeof(INotificationHandler<TestNotification>) &&
            x.ImplementationType == typeof(TestHandler));
    }

    [Fact]
    public void AddOpenMediatR_ShouldRegisterExplicitPipelineBehaviors()
    {
        var services = CreateServices();

        services.Should().Contain(x =>
            x.ServiceType == typeof(IPipelineBehavior<,>) &&
            x.ImplementationType == typeof(TestPipelineBehavior1<,>));

        services.Should().Contain(x =>
            x.ServiceType == typeof(IPipelineBehavior<,>) &&
            x.ImplementationType == typeof(TestPipelineBehavior2<,>));
    }

    [Fact]
    public void AddOpenMediatR_ShouldRegisterCoreServices()
    {
        var services = CreateServices();

        services.Should().Contain(x => x.ServiceType == typeof(ISender) && x.ImplementationType == typeof(OpenMediatRSender));
        services.Should().Contain(x => x.ServiceType == typeof(IPublisher) && x.ImplementationType == typeof(OpenMediatRPublisher));
        services.Should().Contain(x => x.ServiceType == typeof(IMediator) && x.ImplementationType == typeof(Mediator));
        services.Should().Contain(x => x.ServiceType == typeof(INotificationSink) && x.ImplementationType == typeof(InMemoryNotificationSink));
    }

    [Fact]
    public void AddOpenBehavior_WithNonGenericType_ShouldThrow()
    {
        var act = () => CreateServices(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<DependencyInjectionTests>();
            cfg.AddOpenBehavior(typeof(string));
        });

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddOpenBehavior_WithTypeThatDoesNotImplementIPipelineBehavior_ShouldThrow()
    {
        var act = () => CreateServices(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<DependencyInjectionTests>();
            cfg.AddOpenBehavior(typeof(List<>));
        });

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddOpenMediatR_WithNoAssemblies_ShouldThrow()
    {
        var services = new ServiceCollection();

        var act = () => services.AddOpenMediatR(_ => { });

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddOpenMediatR_ShouldNotRegisterNonMediatRInterfaces()
    {
        var services = CreateServices();

        services.Should().NotContain(x => x.ServiceType == typeof(IDisposable));
    }

    [Fact]
    public void AddOpenMediatR_CalledTwice_ShouldNotDuplicateCoreServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

        services.AddOpenMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<DependencyInjectionTests>();
        });

        services.AddOpenMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<DependencyInjectionTests>();
        });

        services.Where(x => x.ServiceType == typeof(ISender)).Should().HaveCount(1);
        services.Where(x => x.ServiceType == typeof(IPublisher)).Should().HaveCount(1);
        services.Where(x => x.ServiceType == typeof(IMediator)).Should().HaveCount(1);
    }
}
