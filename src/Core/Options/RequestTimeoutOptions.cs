namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures the maximum time allowed for processing a request.
/// </summary>
public sealed record RequestTimeoutOptions
{
    /// <summary>
    /// Gets the request timeout, in seconds. Defaults to 100.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 100;
}
