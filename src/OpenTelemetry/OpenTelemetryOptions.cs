namespace Aurore.Foundation.OpenTelemetry;

/// <summary>
/// Top-level configuration for wiring up OpenTelemetry logging, metrics and tracing, including
/// the shared OTLP collector connection settings.
/// </summary>
public sealed record OpenTelemetryOptions
{
    /// <summary>
    /// Gets the URI of the OTLP collector endpoint that logs, metrics and traces are exported to.
    /// </summary>
    public required string CollectorEndpoint { get; init; }

    /// <summary>
    /// Gets the name of the <see cref="global::OpenTelemetry.Exporter.OtlpExportProtocol"/> value used to
    /// communicate with the OTLP collector (e.g. <c>Grpc</c> or <c>HttpProtobuf</c>).
    /// </summary>
    public required string Protocol { get; init; }

    /// <summary>
    /// Gets the raw header string (in the OTLP exporter's <c>key1=value1,key2=value2</c> format)
    /// sent with every export request to the OTLP collector.
    /// </summary>
    public string Headers { get; init; } = string.Empty;

    /// <summary>
    /// Gets the logging-specific OpenTelemetry configuration.
    /// </summary>
    public OpenTelemetryLoggingOptions Logging { get; init; } = new();

    /// <summary>
    /// Gets the metrics-specific OpenTelemetry configuration.
    /// </summary>
    public OpenTelemetryMetricsOptions Metrics { get; init; } = new();

    /// <summary>
    /// Gets the tracing-specific OpenTelemetry configuration.
    /// </summary>
    public OpenTelemetryTracingOptions Tracing { get; init; } = new();
}
