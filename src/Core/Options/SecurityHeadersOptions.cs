namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures HTTP security response headers.
/// </summary>
public sealed record SecurityHeadersOptions
{
    /// <summary>
    /// Gets the value of the <c>Content-Security-Policy</c> header, or <see langword="null"/> to omit it.
    /// Defaults to <c>"default-src 'none'; frame-ancestors 'none'"</c>.
    /// </summary>
    public string? ContentSecurityPolicy { get; init; } = "default-src 'none'; frame-ancestors 'none'";
}
