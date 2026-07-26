using System;
using System.Linq;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aurore.Foundation.AspNetCore.HealthChecks;

internal record SimplifiedHealthCheckReadinessReport
{
    public required HealthStatus Status { get; init; }
    public required TimeSpan TotalDuration { get; init; }
    public static DateTime Timestamp => DateTime.UtcNow;

    public static async Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        var response = new SimplifiedHealthCheckReadinessReport
        {
            Status = GetStatus(report),
            TotalDuration = report.TotalDuration
        };

        await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }

    protected static HealthStatus GetStatus(HealthReport report)
    {
        var status = report.Status;

        if (status == HealthStatus.Unhealthy)
            status = HasCriticalUnhealthy() ? HealthStatus.Unhealthy : HealthStatus.Degraded;

        return status;

        bool HasCriticalUnhealthy()
        {
            return report.Entries.Any(x =>
                x.Value.Tags.Contains(HealthCheckProperties.Tags.Critical) &&
                x.Value.Status is not HealthStatus.Healthy);
        }
    }
}
