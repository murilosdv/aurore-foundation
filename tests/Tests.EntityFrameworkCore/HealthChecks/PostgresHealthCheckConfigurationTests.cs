using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.EntityFrameworkCore.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Aurore.Foundation.Tests.EntityFrameworkCore.HealthChecks;

public class PostgresHealthCheckConfigurationTests
{
    [Fact(DisplayName = "AddPostgresHealthCheck registers a \"postgres\" health check tagged as readiness, database, and critical, failing as Unhealthy")]
    public void RegistersPostgresHealthCheck()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPostgresHealthCheck("Host=localhost;Database=test;Username=test;Password=test");
        using var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value;
        var registration = Assert.Single(options.Registrations, r => r.Name == "postgres");
        Assert.Equal(HealthStatus.Unhealthy, registration.FailureStatus);
        Assert.Contains(HealthCheckProperties.Tags.Readiness, registration.Tags);
        Assert.Contains(HealthCheckProperties.Tags.TypeDatabase, registration.Tags);
        Assert.Contains(HealthCheckProperties.Tags.Critical, registration.Tags);
    }
}
