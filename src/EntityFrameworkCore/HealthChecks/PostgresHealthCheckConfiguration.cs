using System;
using Aurore.Foundation.Core.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Aurore.Foundation.EntityFrameworkCore.HealthChecks;

/// <summary>
/// Provides extension methods for registering a Postgres database health check.
/// </summary>
public static class PostgresHealthCheckConfiguration
{
    /// <summary>
    /// Registers a keyed <see cref="NpgsqlDataSource"/> for the health check and adds a "postgres" health check
    /// that runs a <c>SELECT 1;</c> query, tagged as readiness, database, and critical, with a 1-minute timeout.
    /// </summary>
    /// <param name="services">The service collection to add the health check to.</param>
    /// <param name="connectionString">The Postgres connection string used to create the health check's data source.</param>
    /// <returns>The same <paramref name="services"/> instance, for chaining.</returns>
    public static IServiceCollection AddPostgresHealthCheck(this IServiceCollection services, string connectionString)
    {
        string[] tags = [
            HealthCheckProperties.Tags.Readiness,
            HealthCheckProperties.Tags.TypeDatabase,
            HealthCheckProperties.Tags.Critical,
        ];

        services
            .AddKeyedTransient("hc-postgres", (_, _) => NpgsqlDataSource.Create(connectionString))
            .AddHealthChecks()
            .AddCheck<PostgresHealthCheck>("postgres", HealthStatus.Unhealthy, tags, TimeSpan.FromMinutes(1));

        return services;
    }
}
