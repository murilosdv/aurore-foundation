using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.HealthChecks;
using Aurore.Foundation.Core.Errors;
using Aurore.Foundation.TestBed.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;

namespace Aurore.Foundation.Tests.AspNetCore.HealthChecks;

public class HttpServiceHealthCheckTests
{
    private const string ClientName = "hc-my-service";

    private static (HttpServiceHealthCheck HealthCheck, HealthCheckContext Context) CreateHealthCheck(FakeHttpMessageHandler handler)
    {
        var services = new ServiceCollection();
        services
            .AddHttpClient(ClientName, client => client.BaseAddress = new Uri("https://example.com"))
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var clientFactory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        var healthCheck = new HttpServiceHealthCheck(NullLogger<HttpServiceHealthCheck>.Instance, clientFactory);

        var registration = new HealthCheckRegistration("my-service", healthCheck, HealthStatus.Unhealthy, null);
        var context = new HealthCheckContext { Registration = registration };

        return (healthCheck, context);
    }

    [Fact(DisplayName = "CheckHealthAsync returns Healthy when the dependency responds with a success status code")]
    public async Task ReturnsHealthyForSuccessStatusCode()
    {
        // Arrange
        var handler = FakeHttpMessageHandler.ThatReturns(new HttpResponseMessage(HttpStatusCode.OK));
        var (healthCheck, context) = CreateHealthCheck(handler);

        // Act
        var result = await healthCheck.CheckHealthAsync(context, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact(DisplayName = "CheckHealthAsync returns the registration's failure status when the dependency responds with a non-success status code")]
    public async Task ReturnsFailureStatusForNonSuccessStatusCode()
    {
        // Arrange
        var handler = FakeHttpMessageHandler.ThatReturns(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        var (healthCheck, context) = CreateHealthCheck(handler);

        // Act
        var result = await healthCheck.CheckHealthAsync(context, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(context.Registration.FailureStatus, result.Status);
        Assert.Equal(StandardErrors.BadGateway.Detail, result.Description);
    }

    [Fact(DisplayName = "CheckHealthAsync returns the registration's failure status and the thrown exception when the request fails")]
    public async Task ReturnsFailureStatusAndExceptionWhenRequestThrows()
    {
        // Arrange
        var exception = new HttpRequestException("connection refused");
        var handler = FakeHttpMessageHandler.ThatThrows(exception);
        var (healthCheck, context) = CreateHealthCheck(handler);

        // Act
        var result = await healthCheck.CheckHealthAsync(context, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(context.Registration.FailureStatus, result.Status);
        Assert.Equal(StandardErrors.BadGateway.Detail, result.Description);
        Assert.Same(exception, result.Exception);
    }
}
