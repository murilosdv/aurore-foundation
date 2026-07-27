using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Filters;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Options;
using Aurore.Foundation.TestBed.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.AspNetCore.Filters;

public class IdempotencyFilterTests
{
    private static HybridCache CreateCache()
    {
        return new ServiceCollection()
            .AddHybridCache()
            .Services
            .BuildServiceProvider()
            .GetRequiredService<HybridCache>();
    }

    private static DefaultEndpointFilterInvocationContext CreateInvocationContext(string? idempotencyKey)
    {
        var setup = HttpContextSetup.Create();

        if (idempotencyKey is not null)
            setup.WithHeader(StandardHeaders.IdempotencyKey, idempotencyKey);

        return new DefaultEndpointFilterInvocationContext(setup.Build());
    }

    [Fact(DisplayName = "InvokeAsync calls next directly without caching when no idempotency key header is present")]
    public async Task CallsNextDirectlyWithoutKey()
    {
        // Arrange
        var filter = new IdempotencyFilter(CreateCache(), new IdempotencyOptions());
        var invocationContext = CreateInvocationContext(idempotencyKey: null);
        var callCount = 0;
        EndpointFilterDelegate next = _ =>
        {
            callCount++;
            return ValueTask.FromResult<object?>(Results.Ok("hello"));
        };

        // Act
        var result = await filter.InvokeAsync(invocationContext, next);

        // Assert
        Assert.Equal(1, callCount);
        var statusResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
        Assert.Equal(200, statusResult.StatusCode);
    }

    [Fact(DisplayName = "InvokeAsync caches the first call's result and replays it without invoking next again for a repeated key")]
    public async Task CachesAndReplaysResultForRepeatedKey()
    {
        // Arrange
        var cache = CreateCache();
        var options = new IdempotencyOptions();
        var callCount = 0;
        EndpointFilterDelegate next = _ =>
        {
            callCount++;
            return ValueTask.FromResult<object?>(Results.Ok("hello"));
        };

        // Act
        var firstResult = await new IdempotencyFilter(cache, options).InvokeAsync(CreateInvocationContext("same-key"), next);
        var secondResult = await new IdempotencyFilter(cache, options).InvokeAsync(CreateInvocationContext("same-key"), next);

        // Assert
        Assert.Equal(1, callCount);

        var firstStatus = Assert.IsAssignableFrom<IStatusCodeHttpResult>(firstResult);
        var secondStatus = Assert.IsAssignableFrom<IStatusCodeHttpResult>(secondResult);
        Assert.Equal(firstStatus.StatusCode, secondStatus.StatusCode);

        // The replayed value is round-tripped through HybridCache's serializer, so it comes back
        // as a JsonElement rather than the original string - compare their textual representation.
        var firstValue = Assert.IsAssignableFrom<IValueHttpResult>(firstResult);
        var secondValue = Assert.IsAssignableFrom<IValueHttpResult>(secondResult);
        Assert.Equal(firstValue.Value?.ToString(), secondValue.Value?.ToString());
    }

    [Fact(DisplayName = "InvokeAsync does not cache a result without a status code and returns it directly")]
    public async Task DoesNotCacheResultWithoutStatusCode()
    {
        // Arrange
        var cache = CreateCache();
        var options = new IdempotencyOptions();
        var callCount = 0;
        EndpointFilterDelegate next = _ =>
        {
            callCount++;
            return ValueTask.FromResult<object?>("raw-string-result");
        };

        // Act
        var firstResult = await new IdempotencyFilter(cache, options).InvokeAsync(CreateInvocationContext("not-cacheable-key"), next);
        var secondResult = await new IdempotencyFilter(cache, options).InvokeAsync(CreateInvocationContext("not-cacheable-key"), next);

        // Assert
        Assert.Equal("raw-string-result", firstResult);
        Assert.Equal("raw-string-result", secondResult);
        Assert.Equal(2, callCount);
    }
}
