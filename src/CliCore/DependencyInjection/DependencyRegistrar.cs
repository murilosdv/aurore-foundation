using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Aurore.Foundation.CLI.Core.DependencyInjection;

/// <summary>
/// Bridges an <see cref="IServiceCollection"/> into Spectre.Console.Cli's own type registration
/// pipeline, so commands can receive constructor-injected dependencies. Used by
/// <c>CommandApp.CreateWithServices</c>.
/// </summary>
/// <param name="services">The service collection commands will be resolved from.</param>
public sealed class DependencyRegistrar(IServiceCollection services) : ITypeRegistrar
{
    /// <summary>
    /// Builds the service provider and wraps it as a Spectre.Console.Cli type resolver.
    /// </summary>
    public ITypeResolver Build()
    {
        return new Resolver(services.BuildServiceProvider());
    }

    /// <summary>
    /// Registers an implementation type for a service type.
    /// </summary>
    /// <param name="service">The service type to register.</param>
    /// <param name="implementation">The implementation type to register it with.</param>
    public void Register(Type service, Type implementation)
    {
        services.AddSingleton(service, implementation);
    }

    /// <summary>
    /// Registers an existing instance for a service type.
    /// </summary>
    /// <param name="service">The service type to register.</param>
    /// <param name="implementation">The instance to register.</param>
    public void RegisterInstance(Type service, object implementation)
    {
        services.AddSingleton(service, implementation);
    }

    /// <summary>
    /// Registers a factory that lazily produces the implementation for a service type.
    /// </summary>
    /// <param name="service">The service type to register.</param>
    /// <param name="factory">The factory invoked the first time the service is resolved.</param>
    public void RegisterLazy(Type service, Func<object> factory)
    {
        services.AddSingleton(service, _ => factory());
    }

    private sealed class Resolver(IServiceProvider provider) : ITypeResolver
    {
        public object? Resolve(Type? type)
        {
            return type == null ? null : provider.GetService(type);
        }
    }
}
