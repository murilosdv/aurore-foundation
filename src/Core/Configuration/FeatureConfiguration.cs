using System;
using System.Linq;
using Aurore.Foundation.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Core.Configuration;

/// <summary>
/// Provides dependency-injection registration helpers for <see cref="IFeature{TResult}"/> and <see cref="IFeature{TRequest, TResult}"/> implementations.
/// </summary>
public static class FeatureConfiguration
{
    /// <summary>
    /// Scans the assembly containing <typeparamref name="TAssembly"/> for concrete classes implementing
    /// <see cref="IFeature{TResult}"/> or <see cref="IFeature{TRequest, TResult}"/> and registers each of them with the container.
    /// </summary>
    /// <typeparam name="TAssembly">A marker type whose containing assembly is scanned for feature implementations.</typeparam>
    /// <param name="services">The service collection to register the discovered features into.</param>
    /// <param name="lifetime">The service lifetime to use for each registered feature. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, to allow chaining.</returns>
    public static IServiceCollection AddFeaturesFromAssembly<TAssembly>(
        this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var featureTypes = typeof(TAssembly).Assembly.GetTypes().Where(IsFeature);

        foreach (var featureType in featureTypes)
            services.Add(new ServiceDescriptor(featureType, featureType, lifetime));

        return services;

        static bool IsFeature(Type type)
        {
            if (type is not { IsClass: true, IsAbstract: false })
                return false;

            return type.GetInterfaces().Any(i =>
                i.IsGenericType &&
                (i.GetGenericTypeDefinition() == typeof(IFeature<>) ||
                 i.GetGenericTypeDefinition() == typeof(IFeature<,>)));
        }
    }
}
