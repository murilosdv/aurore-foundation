using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.RateLimiting;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.Core.Options;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;

namespace Aurore.Foundation.AspNetCore.HealthChecks;

/// <summary>
/// Provides extension methods for mapping health check endpoints and configuring their rate limiting and output caching.
/// </summary>
public static class HealthCheckConfiguration
{
    private static readonly Dictionary<HealthStatus, int> ResultStatusCodes = new()
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    };

    /// <summary>
    /// Maps the standard liveness, readiness, and status health check endpoints.
    /// </summary>
    /// <param name="app">The web application to map endpoints on.</param>
    /// <param name="options">The service identity (name, version) and API key used to protect the endpoints.</param>
    /// <returns>The <see cref="WebApplication"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static WebApplication MapHealthCheckEndpoints(this WebApplication app, HealthCheckEndpointOptions options)
    {
        return app
            .MapLivenessEndpoint(options.ServiceName, options.Version)
            .MapReadinessEndpoint(HealthCheckProperties.Endpoints.Readiness, options.ApiKey)
            .MapReadinessEndpoint(HealthCheckProperties.Endpoints.Status, options.ApiKey);
    }

    /// <summary>
    /// Registers a fixed-window rate limiter for health check endpoints, rejecting excess requests with a 429 response.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="permitLimit">The maximum number of requests allowed per window.</param>
    /// <param name="limitSeconds">The length of the rate-limiting window, in seconds.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddHealthCheckRateLimiter(this IServiceCollection services, int permitLimit, int limitSeconds)
    {
        return services
            .AddRateLimiter(o =>
            {
                o.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    await context.HttpContext.Response.WriteAsJsonAsync(new { Message = "My man, take a chill pill. Too many requests." }, cancellationToken);
                };

                o.AddFixedWindowLimiter(HealthCheckProperties.Configuration.RateLimitingKey, limiter =>
                {
                    limiter.PermitLimit = permitLimit;
                    limiter.Window = TimeSpan.FromSeconds(limitSeconds);
                    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiter.QueueLimit = 0;
                });
            });
    }

    /// <summary>
    /// Registers an output cache policy for health check endpoints, varying the cached response by the authorization header.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="expirationSeconds">The number of seconds a cached response remains valid.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddHealthCheckOutputCache(this IServiceCollection services, int expirationSeconds)
    {
        return services
            .AddOutputCache(o => o
                .AddPolicy(HealthCheckProperties.Configuration.OutputCacheKey, policy =>
                {
                    policy.Expire(TimeSpan.FromSeconds(expirationSeconds));
                    policy.SetVaryByHeader(HealthCheckProperties.Headers.AuthorizationKey);
                    policy.Cache();
                }));
    }

    /// <summary>
    /// Maps a readiness-style health check endpoint at the given path, restricted to checks tagged for readiness
    /// (and, for the standard readiness path, critical dependencies only), guarded by the given API key.
    /// </summary>
    /// <param name="app">The web application to map the endpoint on.</param>
    /// <param name="path">The route path to map the endpoint at.</param>
    /// <param name="expectedKey">The API key callers must present in the authorization header to access the endpoint.</param>
    /// <returns>The <see cref="WebApplication"/> so calls can be chained.</returns>
    public static WebApplication MapReadinessEndpoint(this WebApplication app, string path, string expectedKey)
    {
        var criticalOnly = path.IsEqualTo(HealthCheckProperties.Endpoints.Readiness);

        var options = new HealthCheckOptions
        {
            Predicate = check => ContainsReadinessTags(check, criticalOnly),
            AllowCachingResponses = true,
            ResultStatusCodes = ResultStatusCodes,
            ResponseWriter = criticalOnly
                ? SimplifiedHealthCheckReadinessReport.WriteResponseAsync
                : HealthCheckReadinessReport.WriteResponseAsync
        };

        var name = $"HealthCheck{path.Split('/').Last().Humanize()}";

        app.MapHealthChecks(path, options)
            .WithName(name)
            .WithSummary(name.Humanize())
            .CacheOutput(HealthCheckProperties.Configuration.OutputCacheKey)
            .RequireRateLimiting(HealthCheckProperties.Configuration.RateLimitingKey)
            .AddEndpointFilter(async (context, next) =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(HealthCheckConfiguration));

                var hasKey = context.HttpContext.Request.Headers.TryGetValue(HealthCheckProperties.Headers.AuthorizationKey, out var key);

                if (hasKey && key.ToString().IsEqualTo(expectedKey))
                    return await next(context);

                logger.LogWarning("Unauthorized access attempt to readiness endpoint");

                return Results.Unauthorized();
            });

        return app;
    }

    [ExcludeFromCodeCoverage]
    private static WebApplication MapLivenessEndpoint(this WebApplication app, string name, string version)
    {
        var kebabName = name.Kebaberize();

        app
            .MapGet(HealthCheckProperties.Endpoints.Liveness, () => Results.Ok(new
            {
                version,
                Name = kebabName,
                Timestamp = DateTime.UtcNow
            }))
            .ExcludeFromDescription()
            .RequireRateLimiting(HealthCheckProperties.Configuration.RateLimitingKey);

        return app;
    }

    private static bool ContainsReadinessTags(HealthCheckRegistration check, bool onlyCritical)
    {
        var ready = HealthCheckProperties.Tags.Readiness;

        return onlyCritical
            ? check.Tags.Contains(ready) && check.Tags.Contains(HealthCheckProperties.Tags.Critical)
            : check.Tags.Contains(ready);
    }
}
