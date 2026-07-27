using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Middlewares;
using Aurore.Foundation.Core.Options;
using Aurore.Foundation.TestBed.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.AspNetCore.Middlewares;

public class MiddlewareConfigurationTests
{
    private static (RequestDelegate Pipeline, DefaultHttpContext Context, CapturingResponseFeature Feature) BuildPipeline(
        SecurityHeadersOptions options, bool isProduction)
    {
        var services = new ServiceCollection().AddLogging().AddOptions();
        var builder = new ApplicationBuilder(services.BuildServiceProvider());

        builder.UseSecurityHeaders(options, isProduction);

        var feature = new CapturingResponseFeature();
        var context = HttpContextSetup.Create().WithFeature<IHttpResponseFeature>(feature).Build();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");

        return (builder.Build(), context, feature);
    }

    [Fact(DisplayName = "UseSecurityHeaders keeps the configured Content-Security-Policy and enables HSTS in production")]
    public async Task KeepsContentSecurityPolicyAndEnablesHstsInProduction()
    {
        // Arrange
        var options = new SecurityHeadersOptions { ContentSecurityPolicy = "default-src 'self'" };
        var (pipeline, context, feature) = BuildPipeline(options, isProduction: true);

        // Act
        await pipeline(context);
        await feature.FireOnStartingAsync();

        // Assert
        Assert.Equal("default-src 'self'", context.Response.Headers["Content-Security-Policy"]);
        Assert.True(context.Response.Headers.ContainsKey("Strict-Transport-Security"));
    }

    [Fact(DisplayName = "UseSecurityHeaders strips the Content-Security-Policy and does not enable HSTS outside production")]
    public async Task StripsContentSecurityPolicyAndSkipsHstsOutsideProduction()
    {
        // Arrange
        var options = new SecurityHeadersOptions { ContentSecurityPolicy = "default-src 'self'" };
        var (pipeline, context, feature) = BuildPipeline(options, isProduction: false);

        // Act
        await pipeline(context);
        await feature.FireOnStartingAsync();

        // Assert
        Assert.True(string.IsNullOrEmpty(context.Response.Headers["Content-Security-Policy"]));
        Assert.False(context.Response.Headers.ContainsKey("Strict-Transport-Security"));
    }
}
