using System;
using System.Collections.Generic;
using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.AspNetCore.Configuration;

/// <summary>
/// Provides extension methods for configuring Cross-Origin Resource Sharing (CORS) policies.
/// </summary>
public static class CorsConfiguration
{
    /// <summary>
    /// Registers a default CORS policy that allows any header, method, and origin.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddAllowAnyPolicy(this IServiceCollection services)
    {
        return services
            .AddCors(options => options
                .AddDefaultPolicy(builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()));
    }

    /// <summary>
    /// Registers named CORS policies from the given configuration. A policy named <c>"default"</c> is registered as the
    /// default policy; all others are registered under their own name.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="policies">The CORS policies to register.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="policies"/> contains more than one policy named <c>"default"</c>.</exception>
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IEnumerable<CorsPolicyOptions> policies)
    {
        var defaultIsAdded = false;

        return services.AddCors(options =>
        {
            foreach (var policy in policies)
                if (policy.Name.IsEqualTo("default"))
                {
                    if (defaultIsAdded)
                        throw new InvalidOperationException("Only one default policy is allowed.");

                    options.AddDefaultPolicy(BuildPolicy(policy));
                    defaultIsAdded = true;
                }
                else
                {
                    options.AddPolicy(policy.Name, BuildPolicy(policy));
                }
        });

        Action<CorsPolicyBuilder> BuildPolicy(CorsPolicyOptions policy)
        {
            return options =>
            {
                options
                    .WithHeaders(policy.AllowedHeaders)
                    .WithMethods(policy.AllowedMethods)
                    .WithOrigins(policy.AllowedOrigins)
                    .WithExposedHeaders(policy.ExposedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(policy.PreflightMaxAgeSeconds));

                if (policy.AllowCredentials)
                    options.AllowCredentials();
            };
        }
    }
}
