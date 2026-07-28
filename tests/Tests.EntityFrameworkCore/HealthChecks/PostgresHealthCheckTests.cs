using System.Threading.Tasks;
using Aurore.Foundation.EntityFrameworkCore.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Aurore.Foundation.Tests.EntityFrameworkCore.HealthChecks;

public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public string ConnectionString { get; private set; } = string.Empty;

    public async ValueTask InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:18-alpine").Build();

        await _container.StartAsync();

        ConnectionString = _container.GetConnectionString();
    }

    public async ValueTask DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }
}

[CollectionDefinition(nameof(PostgresCollection))]
public sealed class PostgresCollection : ICollectionFixture<PostgresContainerFixture>;

[Collection(nameof(PostgresCollection))]
public class PostgresHealthCheckTests(PostgresContainerFixture fixture)
{
    private static PostgresHealthCheck CreateHealthCheck(string registrationKey, string connectionString)
    {
        var services = new ServiceCollection();
        services.AddKeyedSingleton(registrationKey, (_, _) => NpgsqlDataSource.Create(connectionString));
        var provider = services.BuildServiceProvider();

        return new PostgresHealthCheck(NullLogger<PostgresHealthCheck>.Instance, provider);
    }

    private static HealthCheckContext CreateContext(string registrationName, HealthStatus failureStatus, IHealthCheck healthCheck)
    {
        var registration = new HealthCheckRegistration(registrationName, healthCheck, failureStatus, null);

        return new HealthCheckContext { Registration = registration };
    }

    [Fact(DisplayName = "CheckHealthAsync returns Healthy when the Postgres instance is reachable")]
    public async Task ReturnsHealthyWhenReachable()
    {
        // Arrange
        var healthCheck = CreateHealthCheck("hc-my-db", fixture.ConnectionString);
        var context = CreateContext("my-db", HealthStatus.Unhealthy, healthCheck);

        // Act
        var result = await healthCheck.CheckHealthAsync(context, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact(DisplayName = "CheckHealthAsync returns the registration's failure status and the exception when the Postgres instance is unreachable")]
    public async Task ReturnsFailureStatusWhenUnreachable()
    {
        // Arrange
        var unreachableConnectionString = "Host=127.0.0.1;Port=1;Username=postgres;Password=postgres;Timeout=1";
        var healthCheck = CreateHealthCheck("hc-unreachable-db", unreachableConnectionString);
        var context = CreateContext("unreachable-db", HealthStatus.Degraded, healthCheck);

        // Act
        var result = await healthCheck.CheckHealthAsync(context, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.NotNull(result.Exception);
    }
}
