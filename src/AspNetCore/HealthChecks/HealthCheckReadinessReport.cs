using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aurore.Foundation.AspNetCore.HealthChecks;

internal sealed record HealthCheckReadinessReport : SimplifiedHealthCheckReadinessReport
{
    public IEnumerable<DependencyReport> Dependencies { get; init; } = [];

    public static new async Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        var response = new HealthCheckReadinessReport
        {
            Status = GetStatus(report),
            TotalDuration = report.TotalDuration,
            Dependencies = [.. report.Entries.Select(DependencyReport.From)]
        };

        await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }
}
