namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures the identity presented by a service's health check endpoint.
/// </summary>
public sealed record HealthCheckEndpointOptions
{
    /// <summary>
    /// Gets the name of the service exposing the health check endpoint.
    /// </summary>
    public required string ServiceName { get; init; }

    /// <summary>
    /// Gets the version of the service exposing the health check endpoint.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets the API key required to access the health check endpoint.
    /// </summary>
    public required string ApiKey { get; init; }
}
