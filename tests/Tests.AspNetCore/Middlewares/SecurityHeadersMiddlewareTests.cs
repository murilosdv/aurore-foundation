using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Middlewares;
using Aurore.Foundation.Core.Options;
using Aurore.Foundation.TestBed.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Aurore.Foundation.Tests.AspNetCore.Middlewares;

public class SecurityHeadersMiddlewareTests
{
    private static (DefaultHttpContext Context, CapturingResponseFeature Feature) CreateContext()
    {
        var feature = new CapturingResponseFeature();
        var context = HttpContextSetup.Create().WithFeature<IHttpResponseFeature>(feature).Build();

        return (context, feature);
    }

    [Fact(DisplayName = "InvokeAsync always sets the standard security headers")]
    public async Task AlwaysSetsStandardSecurityHeaders()
    {
        // Arrange
        var (context, feature) = CreateContext();
        var middleware = new SecurityHeadersMiddleware(_ => Task.CompletedTask, new SecurityHeadersOptions());

        // Act
        await middleware.InvokeAsync(context);
        await feature.FireOnStartingAsync();

        // Assert
        Assert.Equal("nosniff", context.Response.Headers["X-Content-Type-Options"]);
        Assert.Equal("DENY", context.Response.Headers["X-Frame-Options"]);
        Assert.Equal("no-referrer", context.Response.Headers["Referrer-Policy"]);
    }

    [Fact(DisplayName = "InvokeAsync sets Content-Security-Policy when configured")]
    public async Task SetsContentSecurityPolicyWhenConfigured()
    {
        // Arrange
        var (context, feature) = CreateContext();
        var options = new SecurityHeadersOptions { ContentSecurityPolicy = "default-src 'self'" };
        var middleware = new SecurityHeadersMiddleware(_ => Task.CompletedTask, options);

        // Act
        await middleware.InvokeAsync(context);
        await feature.FireOnStartingAsync();

        // Assert
        Assert.Equal("default-src 'self'", context.Response.Headers["Content-Security-Policy"]);
    }

    [Fact(DisplayName = "InvokeAsync omits Content-Security-Policy when it is null or empty")]
    public async Task OmitsContentSecurityPolicyWhenNullOrEmpty()
    {
        // Arrange
        var (context, feature) = CreateContext();
        var options = new SecurityHeadersOptions { ContentSecurityPolicy = null };
        var middleware = new SecurityHeadersMiddleware(_ => Task.CompletedTask, options);

        // Act
        await middleware.InvokeAsync(context);
        await feature.FireOnStartingAsync();

        // Assert
        Assert.True(string.IsNullOrEmpty(context.Response.Headers["Content-Security-Policy"]));
    }
}
