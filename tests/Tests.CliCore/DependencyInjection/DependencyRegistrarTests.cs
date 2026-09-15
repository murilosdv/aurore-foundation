using Aurore.Foundation.CliCore.DependencyInjection;
using Aurore.Foundation.Tests.CliCore.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.CliCore.DependencyInjection;

public class DependencyRegistrarTests
{
    [Fact(DisplayName = "Register resolves the registered implementation for the service type")]
    public void RegisterResolvesImplementation()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrar = new DependencyRegistrar(services);
        registrar.Register(typeof(IGreeter), typeof(Greeter));

        // Act
        var resolver = registrar.Build();
        var result = resolver.Resolve(typeof(IGreeter));

        // Assert
        Assert.IsType<Greeter>(result);
    }

    [Fact(DisplayName = "RegisterInstance resolves the exact instance that was registered")]
    public void RegisterInstanceResolvesSameInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrar = new DependencyRegistrar(services);
        var instance = new Greeter();
        registrar.RegisterInstance(typeof(IGreeter), instance);

        // Act
        var resolver = registrar.Build();
        var result = resolver.Resolve(typeof(IGreeter));

        // Assert
        Assert.Same(instance, result);
    }

    [Fact(DisplayName = "RegisterLazy defers factory invocation until the service is actually resolved")]
    public void RegisterLazyDefersFactoryInvocation()
    {
        // Arrange
        var invoked = false;
        var services = new ServiceCollection();
        var registrar = new DependencyRegistrar(services);
        registrar.RegisterLazy(typeof(IGreeter), () =>
        {
            invoked = true;

            return new Greeter();
        });

        // Assert (before resolving)
        Assert.False(invoked);

        // Act
        var resolver = registrar.Build();
        resolver.Resolve(typeof(IGreeter));

        // Assert (after resolving)
        Assert.True(invoked);
    }

    [Fact(DisplayName = "Resolve returns null for a null type")]
    public void ResolveReturnsNullForNullType()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrar = new DependencyRegistrar(services);
        var resolver = registrar.Build();

        // Act
        var result = resolver.Resolve(null);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "Resolve returns null when the requested type was never registered")]
    public void ResolveReturnsNullForUnregisteredType()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrar = new DependencyRegistrar(services);
        var resolver = registrar.Build();

        // Act
        var result = resolver.Resolve(typeof(IGreeter));

        // Assert
        Assert.Null(result);
    }
}
