using System;
using System.Collections.Generic;
using System.Linq;
using Aurore.Foundation.Core.Options;
using Aurore.Foundation.Core.Constants;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aurore.Foundation.AspNetCore.HealthChecks;

/// <summary>
/// Provides extension methods for registering health checks against dependent HTTP services.
/// </summary>
public static class HttpServiceHealthCheckConfiguration
{
    /// <summary>
    /// Registers a named HTTP client and health check for a dependent HTTP service, probing its configured readiness endpoint.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="name">The name under which the health check and HTTP client are registered.</param>
    /// <param name="options">The dependent service's base URL and health check settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddHttpServiceHealthCheck(
        this IServiceCollection services,
        string name,
        HttpServiceDependencyOptions options)
    {
        var opts = new
        {
            Name = name.Kebaberize(),
            options.BaseUrl,
            ReadinessEndpoint = (options.HealthCheck!.ReadinessEndpoint ?? HealthCheckProperties.Endpoints.Readiness).TrimStart('/'),
            Timeout = TimeSpan.FromSeconds(options.HealthCheck!.TimeoutSeconds),
            FailureStatus = options.HealthCheck!.IsCritical ? HealthStatus.Unhealthy : HealthStatus.Degraded,
            Tags = new List<string> {
                HealthCheckProperties.Tags.Readiness,
                HealthCheckProperties.Tags.TypeHttpService,
                options.HealthCheck!.IsCritical ? HealthCheckProperties.Tags.Critical : HealthCheckProperties.Tags.NonCritical
            }.Concat(options.HealthCheck!.Tags.Select(x => x.Kebaberize())),
        };

        services.AddHttpClient($"hc-{opts.Name}", client =>
        {
            client.DefaultRequestHeaders.Add(HealthCheckProperties.Headers.Client, opts.Name);
            client.BaseAddress = new Uri($"{options.BaseUrl}/{opts.ReadinessEndpoint}");
            client.Timeout = opts.Timeout;
        });

        services
            .AddHealthChecks()
            .AddCheck<HttpServiceHealthCheck>(name, opts.FailureStatus, opts.Tags, opts.Timeout);

        return services;
    }
}
