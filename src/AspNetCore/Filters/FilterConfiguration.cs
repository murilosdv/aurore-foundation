using System.Diagnostics.CodeAnalysis;
using Aurore.Foundation.Core.Contexts;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.AspNetCore.Filters;

/// <summary>
/// Provides extension methods for registering and applying endpoint validation and idempotency filters.
/// </summary>
[ExcludeFromCodeCoverage]
public static class FilterConfiguration
{
    /// <summary>
    /// Adds an endpoint filter that validates request arguments using their registered FluentValidation validators,
    /// returning a validation-error response when validation fails.
    /// </summary>
    /// <param name="builder">The route handler builder to configure.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> so calls can be chained.</returns>
    public static RouteHandlerBuilder WithEndpointValidation(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<EndpointValidationFilter>();
    }

    /// <summary>
    /// Adds an endpoint filter that caches and replays the response for requests carrying the same idempotency key.
    /// Only successful (2xx) results are cached — error results are never replayed, so a retry after a transient
    /// failure still reaches the handler. This is a best-effort HTTP-layer replay for cheap, side-effect-free work;
    /// it is not a durable guarantee (a cache eviction, restart, or additional instance can all cause the handler
    /// to run again for the same key), so endpoints whose side effects must never run twice should enforce that
    /// themselves — e.g. a dedup check in the same transaction as the write, keyed off <see cref="RequestContext.IdempotencyKey"/> —
    /// rather than relying on this filter alone.
    /// </summary>
    /// <param name="builder">The route handler builder to configure.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> so calls can be chained.</returns>
    public static RouteHandlerBuilder WithIdempotency(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<IdempotencyFilter>();
    }

    /// <summary>
    /// Registers the hybrid cache and settings required by the idempotency endpoint filter.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">The idempotency cache settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddIdempotency(this IServiceCollection services, IdempotencyOptions options)
    {
        return services
            .AddHybridCache()
            .Services
            .AddSingleton(options);
    }
}
