namespace Aurore.Foundation.OpenTelemetry;

/// <summary>
/// Configures how OpenTelemetry collects and exports metrics.
/// </summary>
public sealed record OpenTelemetryMetricsOptions
{
    /// <summary>
    /// Gets a value indicating whether metrics are additionally exported to the console.
    /// </summary>
    public bool ExportToConsole { get; init; } = false;
}
