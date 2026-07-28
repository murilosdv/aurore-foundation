using System;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Contexts;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;

namespace Aurore.Foundation.AspNetCore.Filters;

internal sealed class IdempotencyFilter(HybridCache cache, IdempotencyOptions options, RequestContext requestContext) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var key = requestContext.IdempotencyKey;

        if (key is null)
            return await next(context);

        var request = context.HttpContext.Request;
        var cacheKey = $"{StandardHeaders.IdempotencyKey}:{request.Method}:{request.Path}:{key}";
        var cancellationToken = context.HttpContext.RequestAborted;

        try
        {
            var entry = await cache.GetOrCreateAsync(
                cacheKey,
                (context, next),
                static async (state, ct) =>
                {
                    var result = await state.next(state.context);

                    // Only successful (2xx) results are cached. Results without a status code (rare custom
                    // IResult implementations) can't be replayed faithfully, and error results (4xx/5xx) are
                    // often transient — caching one would replay the same failure to every retry with this key
                    // for the rest of the cache duration instead of letting a retry actually try again.
                    // Throwing here — rather than returning a sentinel — stops GetOrCreateAsync from persisting
                    // anything, since a factory exception is propagated but never written to the cache.
                    if (result is not IStatusCodeHttpResult { StatusCode: { } statusCode } || statusCode is < 200 or >= 300)
                        throw new NotCacheableResultException(result);

                    return new IdempotencyCacheEntry(statusCode, (result as IValueHttpResult)?.Value);
                },
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(options.CacheDurationMinutes) },
                cancellationToken: cancellationToken);

            return entry.Value is null
                ? Results.StatusCode(entry.StatusCode)
                : Results.Json(entry.Value, statusCode: entry.StatusCode);
        }
        catch (NotCacheableResultException ex)
        {
            return ex.Result;
        }
    }

    private sealed record IdempotencyCacheEntry(int StatusCode, object? Value);

    private sealed class NotCacheableResultException(object? result) : Exception
    {
        public object? Result { get; } = result;
    }
}
