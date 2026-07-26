using System;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Extensions;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;

namespace Aurore.Foundation.AspNetCore.Filters;

internal sealed class IdempotencyFilter(HybridCache cache, IdempotencyOptions options) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var key = context.HttpContext.GetRequestHeader(StandardHeaders.IdempotencyKey);

        if (key.HasValue() is false)
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

                    // Results without a status code (rare custom IResult implementations) can't be
                    // replayed faithfully, so they're never cached. Throwing here — rather than
                    // returning a sentinel — stops GetOrCreateAsync from persisting anything, since
                    // a factory exception is propagated but never written to the cache.
                    if (result is not IStatusCodeHttpResult { StatusCode: { } statusCode })
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
