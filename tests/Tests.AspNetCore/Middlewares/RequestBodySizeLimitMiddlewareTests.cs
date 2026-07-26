using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Middlewares;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Http.Features;
using Tests.Shared.Fakes;
using Tests.Shared.Http;

namespace Tests.AspNetCore.Middlewares;

public class RequestBodySizeLimitMiddlewareTests
{
    [Fact(DisplayName = "InvokeAsync sets MaxRequestBodySize from options when the feature is present and writable")]
    public async Task SetsMaxRequestBodySizeWhenFeatureIsWritable()
    {
        // Arrange
        var feature = new FakeMaxRequestBodySizeFeature { IsReadOnly = false };
        var context = HttpContextSetup.Create().WithFeature<IHttpMaxRequestBodySizeFeature>(feature).Build();
        var middleware = new RequestBodySizeLimitMiddleware(_ => Task.CompletedTask, new RequestBodySizeOptions { MaxBytes = 1024 });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(1024, feature.MaxRequestBodySize);
    }

    [Fact(DisplayName = "InvokeAsync leaves the feature untouched when it is read-only")]
    public async Task LeavesFeatureUntouchedWhenReadOnly()
    {
        // Arrange
        var feature = new FakeMaxRequestBodySizeFeature { IsReadOnly = true, MaxRequestBodySize = 2048 };
        var context = HttpContextSetup.Create().WithFeature<IHttpMaxRequestBodySizeFeature>(feature).Build();
        var middleware = new RequestBodySizeLimitMiddleware(_ => Task.CompletedTask, new RequestBodySizeOptions { MaxBytes = 1024 });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(2048, feature.MaxRequestBodySize);
    }

    [Fact(DisplayName = "InvokeAsync runs next without error when the feature is absent")]
    public async Task RunsNextWhenFeatureIsAbsent()
    {
        // Arrange
        var context = HttpContextSetup.Create().Build();
        var nextCalled = false;
        var middleware = new RequestBodySizeLimitMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, new RequestBodySizeOptions { MaxBytes = 1024 });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
    }
}
