using System;
using System.Linq;
using System.Net.Http;
using Aurore.Foundation.AspNetCore.HealthChecks;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Aurore.Foundation.Tests.AspNetCore.HealthChecks;

public class HttpServiceHealthCheckConfigurationTests
{
    private static HealthCheckRegistration GetRegistration(IServiceCollection services, string name)
    {
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value;

        return options.Registrations.Single(r => r.Name == name);
    }

    [Fact(DisplayName = "AddHttpServiceHealthCheck registers a critical dependency with Unhealthy failure status and the critical tag")]
    public void RegistersCriticalDependencyAsUnhealthy()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new HttpServiceDependencyOptions
        {
            BaseUrl = "https://payments.internal",
            HealthCheck = new HttpServiceHealthCheckOptions { IsCritical = true, TimeoutSeconds = 5 }
        };

        // Act
        services.AddHttpServiceHealthCheck("payments-service", options);
        var registration = GetRegistration(services, "payments-service");

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, registration.FailureStatus);
        Assert.Contains(HealthCheckProperties.Tags.Critical, registration.Tags);
        Assert.DoesNotContain(HealthCheckProperties.Tags.NonCritical, registration.Tags);
        Assert.Contains(HealthCheckProperties.Tags.Readiness, registration.Tags);
        Assert.Contains(HealthCheckProperties.Tags.TypeHttpService, registration.Tags);
    }

    [Fact(DisplayName = "AddHttpServiceHealthCheck registers a non-critical dependency with Degraded failure status and the non-critical tag")]
    public void RegistersNonCriticalDependencyAsDegraded()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new HttpServiceDependencyOptions
        {
            BaseUrl = "https://reports.internal",
            HealthCheck = new HttpServiceHealthCheckOptions { IsCritical = false, TimeoutSeconds = 5 }
        };

        // Act
        services.AddHttpServiceHealthCheck("reports-service", options);
        var registration = GetRegistration(services, "reports-service");

        // Assert
        Assert.Equal(HealthStatus.Degraded, registration.FailureStatus);
        Assert.Contains(HealthCheckProperties.Tags.NonCritical, registration.Tags);
        Assert.DoesNotContain(HealthCheckProperties.Tags.Critical, registration.Tags);
    }

    [Fact(DisplayName = "AddHttpServiceHealthCheck includes the dependency's own tags, kebab-cased, alongside the standard tags")]
    public void IncludesCustomTagsKebabCased()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new HttpServiceDependencyOptions
        {
            BaseUrl = "https://billing.internal",
            HealthCheck = new HttpServiceHealthCheckOptions { IsCritical = true, TimeoutSeconds = 5, Tags = ["BillingTeam"] }
        };

        // Act
        services.AddHttpServiceHealthCheck("billing-service", options);
        var registration = GetRegistration(services, "billing-service");

        // Assert
        Assert.Contains("billing-team", registration.Tags);
    }

    [Fact(DisplayName = "AddHttpServiceHealthCheck configures the named HTTP client with the dependency's base address and timeout")]
    public void ConfiguresHttpClientBaseAddressAndTimeout()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new HttpServiceDependencyOptions
        {
            BaseUrl = "https://orders.internal",
            HealthCheck = new HttpServiceHealthCheckOptions { IsCritical = true, TimeoutSeconds = 7, ReadinessEndpoint = "health/ready" }
        };

        // Act
        services.AddHttpServiceHealthCheck("Orders Service", options);
        var clientFactory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        var client = clientFactory.CreateClient("hc-orders-service");

        // Assert
        Assert.Equal(new Uri("https://orders.internal/health/ready"), client.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(7), client.Timeout);
        Assert.Equal("orders-service", client.DefaultRequestHeaders.GetValues(HealthCheckProperties.Headers.Client).Single());
    }
}
