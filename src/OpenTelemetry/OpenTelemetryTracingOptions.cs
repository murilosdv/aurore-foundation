namespace Aurore.Foundation.OpenTelemetry;

/// <summary>
/// Configures how OpenTelemetry samples, filters and exports distributed traces.
/// </summary>
public sealed record OpenTelemetryTracingOptions
{
    /// <summary>
    /// Gets the sampling ratio (between 0.0 and 1.0) used by the parent-based, trace-ID-ratio sampler
    /// to decide which root traces are recorded.
    /// </summary>
    public double TraceIdRatioBasedSampler { get; init; } = 1.0;

    /// <summary>
    /// Gets the collection of request path prefixes (in addition to the built-in health-check path)
    /// that are excluded from incoming (ASP.NET Core) traffic instrumentation.
    /// </summary>
    public string[] IncomingTrafficFilters { get; init; } = [];

    /// <summary>
    /// Gets the collection of request path prefixes (in addition to the built-in health-check path)
    /// that are excluded from outgoing (<see cref="System.Net.Http.HttpClient"/>) traffic instrumentation.
    /// </summary>
    public string[] OutgoingTrafficFilters { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether traces are additionally exported to the console.
    /// </summary>
    public bool ExportToConsole { get; init; } = false;
}
