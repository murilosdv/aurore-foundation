using System.Collections.Generic;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures an HTTP service dependency, including its base address, default headers, and optional health check.
/// </summary>
public sealed record HttpServiceDependencyOptions
{
    /// <summary>
    /// Gets the base URL of the dependent HTTP service.
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Gets the default headers sent with every request to the dependent service.
    /// </summary>
    public Dictionary<string, object> RequestHeaders { get; init; } = [];

    /// <summary>
    /// Gets the health check configuration for the dependent service, or <see langword="null"/> if health checking is not configured.
    /// </summary>
    public HttpServiceHealthCheckOptions? HealthCheck { get; init; }
}
