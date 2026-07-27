using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures a named Cross-Origin Resource Sharing (CORS) policy.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record CorsPolicyOptions
{
    /// <summary>
    /// Gets the name under which this policy is registered.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the request headers allowed for cross-origin requests.
    /// </summary>
    public string[] AllowedHeaders { get; init; } = [];

    /// <summary>
    /// Gets the HTTP methods allowed for cross-origin requests.
    /// </summary>
    public string[] AllowedMethods { get; init; } = [];

    /// <summary>
    /// Gets the origins allowed to make cross-origin requests.
    /// </summary>
    public string[] AllowedOrigins { get; init; } = [];

    /// <summary>
    /// Gets the response headers exposed to cross-origin callers.
    /// </summary>
    public string[] ExposedHeaders { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether credentials (cookies, authorization headers) are allowed on cross-origin requests.
    /// </summary>
    public bool AllowCredentials { get; init; }

    /// <summary>
    /// Gets the number of seconds a preflight response can be cached by the client.
    /// </summary>
    public int PreflightMaxAgeSeconds { get; init; }
}
