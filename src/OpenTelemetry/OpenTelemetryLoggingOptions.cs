namespace Aurore.Foundation.OpenTelemetry;

/// <summary>
/// Configures how OpenTelemetry captures and exports log records.
/// </summary>
public sealed record OpenTelemetryLoggingOptions
{
    /// <summary>
    /// Gets a value indicating whether logger scopes are included as attributes on emitted log records.
    /// </summary>
    public bool IncludeScopes { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether structured log state values are parsed into individual attributes.
    /// </summary>
    public bool ParseStateValues { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether log records are additionally exported to the console.
    /// </summary>
    public bool AddConsoleExporter { get; init; } = false;
}
