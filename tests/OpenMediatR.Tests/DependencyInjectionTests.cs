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
                x.ImplementationType == typeof(TestRequestHandler))
            .Should().NotBeNull();
    }
}