namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Aggregates the options that configure the HTTP request processing pipeline.
/// </summary>
public sealed record RequestPipelineOptions
{
    /// <summary>
    /// Gets the request timeout configuration.
    /// </summary>
    public RequestTimeoutOptions Timeout { get; init; } = new();

    /// <summary>
    /// Gets the request body size configuration.
    /// </summary>
    public RequestBodySizeOptions BodySize { get; init; } = new();

    /// <summary>
    /// Gets the forwarded headers configuration.
    /// </summary>
    public ForwardedHeadersOptions ForwardedHeaders { get; init; } = new();

    /// <summary>
    /// Gets the Kestrel server limits configuration.
    /// </summary>
    public KestrelLimitsOptions KestrelLimits { get; init; } = new();

    /// <summary>
    /// Gets the security headers configuration.
    /// </summary>
    public SecurityHeadersOptions SecurityHeaders { get; init; } = new();

    /// <summary>
    /// Gets the idempotency handling configuration.
    /// </summary>
    public IdempotencyOptions Idempotency { get; init; } = new();
}
