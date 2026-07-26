using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.AspNetCore.ErrorHandling;

/// <summary>
/// Provides extension methods for registering RFC 7807 problem-details error handling.
/// </summary>
public static class ErrorHandlingConfiguration
{
    /// <summary>
    /// Registers the unhandled-exception handler and problem-details middleware, stripping the default <c>traceId</c>
    /// extension since correlation and trace identifiers are already carried on every response.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddUnexpectedErrorHandler(this IServiceCollection services)
    {
        return services
            .AddExceptionHandler<UnexpectedExceptionHandler>()
            .AddProblemDetails(options =>
            {
                // Correlation-Id and traceparent are already on every response via CorrelationMiddleware —
                // don't duplicate them into the body, and strip ASP.NET Core's default traceId extension.
                options.CustomizeProblemDetails = context => context.ProblemDetails.Extensions.Remove("traceId");
            });
    }

    /// <summary>
    /// Adds the exception-handling middleware to the request pipeline, converting unhandled exceptions into problem-details responses.
    /// </summary>
    /// <param name="builder">The application builder to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so calls can be chained.</returns>
    public static IApplicationBuilder UseUnexpectedErrorHandler(this IApplicationBuilder builder)
    {
        return builder.UseExceptionHandler();
    }
}
