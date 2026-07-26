using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Aurore.Foundation.EntityFrameworkCore.HealthChecks;

/// <summary>
/// An <see cref="IHealthCheck"/> that verifies connectivity to a Postgres instance by executing
/// a <c>SELECT 1;</c> query against the keyed <see cref="Npgsql.NpgsqlDataSource"/> registered
/// for the health check's registration name.
/// </summary>
/// <param name="logger">Logger used to record connection failures.</param>
/// <param name="serviceProvider">Service provider used to resolve the keyed <see cref="Npgsql.NpgsqlDataSource"/> for the check.</param>
internal sealed class PostgresHealthCheck(
    ILogger<PostgresHealthCheck> logger, IServiceProvider serviceProvider) : IHealthCheck
{
    /// <summary>
    /// Executes a <c>SELECT 1;</c> query against the Postgres instance to determine its health.
    /// </summary>
    /// <param name="context">The health check context, whose registration name identifies the keyed data source to use.</param>
    /// <param name="cancellationToken">A token used to observe cancellation requests.</param>
    /// <returns>
    /// A healthy result if the query returns <c>1</c>; otherwise an unhealthy result,
    /// or a result with the registration's configured failure status if an exception occurs.
    /// </returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            await using var dataSource = serviceProvider.GetRequiredKeyedService<NpgsqlDataSource>($"hc-{context.Registration.Name}");

            var connection = dataSource.CreateConnection();

            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            await using var command = connection.CreateCommand();

            command.CommandText = "SELECT 1;";

            var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

            return result is 1
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("Postgres database is unavailable or unreachable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while trying to reach postgres instance.");

            return new HealthCheckResult(context.Registration.FailureStatus, "An error occurred while trying to connect to the Postgres instance.", ex);
        }
    }
}
