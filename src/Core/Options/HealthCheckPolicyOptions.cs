using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures rate limiting and caching policies applied to health check endpoints.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record HealthCheckPolicyOptions
{
    /// <summary>
    /// Gets the number of requests permitted within the rate-limit window. Defaults to 5.
    /// </summary>
    public int RateLimitPermitCount { get; init; } = 5;

    /// <summary>
    /// Gets the length, in seconds, of the rate-limit window. Defaults to 10.
    /// </summary>
    public int RateLimitWindowSeconds { get; init; } = 10;

    /// <summary>
    /// Gets the number of seconds a readiness check response is cached. Defaults to 10.
    /// </summary>
    public int ReadinessCacheSeconds { get; init; } = 10;
}
