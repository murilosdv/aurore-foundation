using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures the trusted proxies and networks allowed to set forwarded headers (e.g. <c>X-Forwarded-For</c>).
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record ForwardedHeadersOptions
{
    /// <summary>
    /// Gets the IP addresses of known, trusted reverse proxies.
    /// </summary>
    public string[] KnownProxies { get; init; } = [];

    /// <summary>
    /// Gets the CIDR network ranges of known, trusted reverse proxies.
    /// </summary>
    public string[] KnownNetworks { get; init; } = [];
}
