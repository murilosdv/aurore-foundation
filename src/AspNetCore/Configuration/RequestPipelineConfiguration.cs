using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Aurore.Foundation.Core.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.AspNetCore.Configuration;

/// <summary>
/// Provides extension methods for configuring request-processing pipeline limits: timeouts, body size, forwarded headers, and Kestrel limits.
/// </summary>
public static class RequestPipelineConfiguration
{
    /// <summary>
    /// Registers a default request timeout policy using the given timeout duration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">The request timeout settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddRequestTimeouts(this IServiceCollection services, RequestTimeoutOptions options)
    {
        return services.AddRequestTimeouts(o =>
            o.DefaultPolicy = new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
            {
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds),
            });
    }

    /// <summary>
    /// Registers the maximum allowed request body size for later use by the request body size limit middleware.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">The request body size settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddRequestBodySizeLimit(this IServiceCollection services, RequestBodySizeOptions options)
    {
        return services.AddSingleton(options);
    }

    /// <summary>
    /// Configures forwarded headers processing (<c>X-Forwarded-For</c>, <c>X-Forwarded-Proto</c>, <c>X-Forwarded-Host</c>) restricted to the given trusted proxies and networks.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">The trusted proxies and networks allowed to set forwarded headers.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddForwardedHeaders(this IServiceCollection services, ForwardedHeadersOptions options)
    {
        return services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(o =>
        {
            o.ForwardedHeaders =
                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto |
                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost;

            foreach (var proxy in options.KnownProxies)
                o.KnownProxies.Add(IPAddress.Parse(proxy));

            foreach (var network in options.KnownNetworks)
                o.KnownIPNetworks.Add(IPNetwork.Parse(network));
        });
    }

    /// <summary>
    /// Configures Kestrel server connection and request limits.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">The connection and request limits to apply.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddKestrelLimits(this IServiceCollection services, KestrelLimitsOptions options)
    {
        return services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(o =>
        {
            o.Limits.MaxConcurrentConnections = options.MaxConcurrentConnections;
            o.Limits.MaxRequestHeaderCount = options.MaxRequestHeaderCount;
            o.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(options.RequestHeadersTimeoutSeconds);
            o.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(options.KeepAliveTimeoutSeconds);
        });
    }

}
