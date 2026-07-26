using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Aurore.Foundation.AspNetCore.MinimalApis;

/// <summary>
/// Provides extension methods for declaring possible problem-details responses on a minimal API route.
/// </summary>
public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Declares that the route may produce a problem-details response for each of the given HTTP status codes.
    /// </summary>
    /// <param name="builder">The route handler builder to configure.</param>
    /// <param name="statusCodes">The HTTP status codes to declare as possible problem responses.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> so calls can be chained.</returns>
    public static RouteHandlerBuilder ProducesProblems(this RouteHandlerBuilder builder, params int[] statusCodes)
    {
        foreach (var statusCode in statusCodes)
            builder.ProducesProblem(statusCode);

        return builder;
    }
}
