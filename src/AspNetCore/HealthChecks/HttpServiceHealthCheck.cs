using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Errors;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Aurore.Foundation.AspNetCore.HealthChecks;

internal sealed class HttpServiceHealthCheck(ILogger<HttpServiceHealthCheck> logger, IHttpClientFactory clientFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpClient = clientFactory.CreateClient($"hc-{context.Registration.Name}");

            var response = await httpClient.GetAsync(string.Empty, cancellationToken).ConfigureAwait(false);

            return response.IsSuccessStatusCode ? HealthCheckResult.Healthy() : Failure();
        }
        catch (Exception ex)
        {
            return Failure(ex);
        }

        HealthCheckResult Failure(Exception? ex = null)
        {
            logger.LogError(ex, "Error while trying to reach service {ServiceName}.", context.Registration.Name);

            return new HealthCheckResult(context.Registration.FailureStatus, StandardErrors.BadGateway.Detail, ex);
        }
    }
}
