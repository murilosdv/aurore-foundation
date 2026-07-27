using System;
using System.Net.Http;
using Aurore.Foundation.Core.Options;
using Aurore.Foundation.OpenTelemetry;
using Aurore.Foundation.TestBed.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;

namespace Aurore.Foundation.Tests.OpenTelemetry;

public class OpenTelemetryExtensionsTests
{
    private static AppInfoOptions CreateAppInfo()
    {
        return new AppInfoOptions
        {
            Version = "1.0.0",
            Domain = "test-domain",
            Boundary = "test-boundary",
            Component = "test-component",
            Description = "A test application used to exercise OpenTelemetry wiring.",
            Maintainer = new MaintainerOptions("Test Maintainer", "maintainer@example.com"),
        };
    }

    private static OpenTelemetryOptions CreateSettings()
    {
        return new OpenTelemetryOptions
        {
            CollectorEndpoint = "http://localhost:4317",
            Protocol = "Grpc",
            Tracing = new OpenTelemetryTracingOptions
            {
                IncomingTrafficFilters = ["/internal"],
                OutgoingTrafficFilters = ["/private"],
            },
        };
    }

    private static Func<HttpContext, bool> GetIncomingFilter()
    {
        var services = new ServiceCollection();
        services.AddDefaultOpenTelemetry(CreateAppInfo(), CreateSettings(), "test-activity-source", "test-environment");
        var provider = services.BuildServiceProvider();

        return provider.GetRequiredService<IOptionsMonitor<AspNetCoreTraceInstrumentationOptions>>().CurrentValue.Filter!;
    }

    private static Func<HttpRequestMessage, bool> GetOutgoingFilter()
    {
        var services = new ServiceCollection();
        services.AddDefaultOpenTelemetry(CreateAppInfo(), CreateSettings(), "test-activity-source", "test-environment");
        var provider = services.BuildServiceProvider();

        return provider.GetRequiredService<IOptionsMonitor<HttpClientTraceInstrumentationOptions>>().CurrentValue.FilterHttpRequestMessage!;
    }

    [Fact(DisplayName = "Incoming traffic filter excludes the built-in /health/ path even though it is not user-configured")]
    public void IncomingFilterExcludesBuiltInHealthPath()
    {
        // Arrange
        var filter = GetIncomingFilter();
        var httpContext = HttpContextSetup.Create().WithPath("/health/live").Build();

        // Act
        var isIncluded = filter(httpContext);

        // Assert
        Assert.False(isIncluded);
    }

    [Fact(DisplayName = "Incoming traffic filter excludes the built-in health path regardless of casing")]
    public void IncomingFilterExcludesHealthPathCaseInsensitively()
    {
        // Arrange
        var filter = GetIncomingFilter();
        var httpContext = HttpContextSetup.Create().WithPath("/HEALTH/live").Build();

        // Act
        var isIncluded = filter(httpContext);

        // Assert
        Assert.False(isIncluded);
    }

    [Fact(DisplayName = "Incoming traffic filter excludes paths configured in IncomingTrafficFilters")]
    public void IncomingFilterExcludesUserConfiguredPath()
    {
        // Arrange
        var filter = GetIncomingFilter();
        var httpContext = HttpContextSetup.Create().WithPath("/internal/status").Build();

        // Act
        var isIncluded = filter(httpContext);

        // Assert
        Assert.False(isIncluded);
    }

    [Fact(DisplayName = "Incoming traffic filter does not exclude unrelated paths")]
    public void IncomingFilterDoesNotExcludeUnrelatedPath()
    {
        // Arrange
        var filter = GetIncomingFilter();
        var httpContext = HttpContextSetup.Create().WithPath("/api/orders").Build();

        // Act
        var isIncluded = filter(httpContext);

        // Assert
        Assert.True(isIncluded);
    }

    [Fact(DisplayName = "Outgoing traffic filter excludes the built-in /health/ path even though it is not user-configured")]
    public void OutgoingFilterExcludesBuiltInHealthPath()
    {
        // Arrange
        var filter = GetOutgoingFilter();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/health/live");

        // Act
        var isIncluded = filter(request);

        // Assert
        Assert.False(isIncluded);
    }

    [Fact(DisplayName = "Outgoing traffic filter excludes the built-in health path regardless of casing")]
    public void OutgoingFilterExcludesHealthPathCaseInsensitively()
    {
        // Arrange
        var filter = GetOutgoingFilter();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/HEALTH/live");

        // Act
        var isIncluded = filter(request);

        // Assert
        Assert.False(isIncluded);
    }

    [Fact(DisplayName = "Outgoing traffic filter excludes paths configured in OutgoingTrafficFilters")]
    public void OutgoingFilterExcludesUserConfiguredPath()
    {
        // Arrange
        var filter = GetOutgoingFilter();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/private/data");

        // Act
        var isIncluded = filter(request);

        // Assert
        Assert.False(isIncluded);
    }

    [Fact(DisplayName = "Outgoing traffic filter does not exclude unrelated paths")]
    public void OutgoingFilterDoesNotExcludeUnrelatedPath()
    {
        // Arrange
        var filter = GetOutgoingFilter();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/api/orders");

        // Act
        var isIncluded = filter(request);

        // Assert
        Assert.True(isIncluded);
    }

    [Fact(DisplayName = "Outgoing traffic filter excludes requests with a null RequestUri")]
    public void OutgoingFilterExcludesNullRequestUri()
    {
        // Arrange
        var filter = GetOutgoingFilter();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/api/orders")
        {
            RequestUri = null,
        };

        // Act
        var isIncluded = filter(request);

        // Assert
        Assert.False(isIncluded);
    }
}
