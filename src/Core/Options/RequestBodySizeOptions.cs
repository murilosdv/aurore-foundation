namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures the maximum allowed size of an incoming request body.
/// </summary>
public sealed record RequestBodySizeOptions
{
    /// <summary>
    /// Gets the maximum request body size, in bytes. Defaults to 10,485,760 (10 MiB).
    /// </summary>
    public long MaxBytes { get; init; } = 10_485_760;
}
