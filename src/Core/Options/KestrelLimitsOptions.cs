using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures Kestrel server connection and request limits.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record KestrelLimitsOptions
{
    /// <summary>
    /// Gets the maximum number of concurrent connections allowed, or <see langword="null"/> for no limit.
    /// </summary>
    public int? MaxConcurrentConnections { get; init; }

    /// <summary>
    /// Gets the maximum number of request headers allowed per request. Defaults to 100.
    /// </summary>
    public int MaxRequestHeaderCount { get; init; } = 100;

    /// <summary>
    /// Gets the number of seconds allowed for the request headers to be received. Defaults to 30.
    /// </summary>
    public int RequestHeadersTimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Gets the number of seconds a keep-alive connection is held open while idle. Defaults to 130.
    /// </summary>
    public int KeepAliveTimeoutSeconds { get; init; } = 130;
}
