using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures how a dependent HTTP service's health is checked.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record HttpServiceHealthCheckOptions
{
    /// <summary>
    /// Gets the relative or absolute readiness endpoint to probe, or <see langword="null"/> if none is configured.
    /// </summary>
    public string? ReadinessEndpoint { get; init; }

    /// <summary>
    /// Gets the number of seconds allowed for the health check request to complete.
    /// </summary>
    public int TimeoutSeconds { get; init; }

    /// <summary>
    /// Gets a value indicating whether this dependency is critical to the overall service's health status.
    /// </summary>
    public bool IsCritical { get; init; }

    /// <summary>
    /// Gets the tags applied to this health check registration.
    /// </summary>
    public List<string> Tags { get; init; } = [];
}
