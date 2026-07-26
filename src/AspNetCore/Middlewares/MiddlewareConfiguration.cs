using Aurore.Foundation.Core.Contexts;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.AspNetCore.Middlewares;

/// <summary>
/// Provides extension methods for registering and applying Aurore's request pipeline middleware.
/// </summary>
public static class MiddlewareConfiguration
{
    /// <summary>
    /// Registers the scoped <see cref="CorrelationContext"/> used to carry correlation and trace identifiers for the current request.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddCorrelationContext(this IServiceCollection services)
    {
        return services
            .AddScoped<CorrelationContext>();
    }

    /// <summary>
    /// Adds middleware that resolves or generates a correlation identifier for the request, populates the
    /// <see cref="CorrelationContext"/>, enriches the logging scope, and echoes the identifier (and trace parent) on the response.
    /// </summary>
    /// <param name="builder">The application builder to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so calls can be chained.</returns>
    public static IApplicationBuilder UseCorrelationContext(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<CorrelationMiddleware>();
    }

    /// <summary>
    /// Adds middleware that enriches the logging scope with standard request attributes (host, user agent, remote IP).
    /// </summary>
    /// <param name="builder">The application builder to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so calls can be chained.</returns>
    public static IApplicationBuilder UseRequestEnricher(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<RequestEnricherMiddleware>();
    }

    /// <summary>
    /// Adds middleware that enforces the configured maximum request body size.
    /// </summary>
    /// <param name="builder">The application builder to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so calls can be chained.</returns>
    public static IApplicationBuilder UseRequestBodySizeLimit(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<RequestBodySizeLimitMiddleware>();
    }

    /// <summary>
    /// Adds middleware that sets standard security response headers. In production, HSTS is also enabled; outside
    /// production, the configured Content-Security-Policy is omitted so local tooling (e.g. Swagger/Scalar UI) keeps working.
    /// </summary>
    /// <param name="builder">The application builder to configure.</param>
    /// <param name="options">The security header values to apply.</param>
    /// <param name="isProduction">Whether the application is running in production.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so calls can be chained.</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder, SecurityHeadersOptions options, bool isProduction)
    {
        if (isProduction)
            builder.UseHsts();

        var effectiveOptions = isProduction ? options : options with { ContentSecurityPolicy = null };

        return builder
            .UseMiddleware<SecurityHeadersMiddleware>(effectiveOptions);
    }

    /// <summary>
    /// Adds middleware that short-circuits requests with a 503 Service Unavailable response while maintenance mode is enabled,
    /// bypassing health check endpoints.
    /// </summary>
    /// <param name="builder">The application builder to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so calls can be chained.</returns>
    public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<MaintenanceModeMiddleware>();
    }
}
