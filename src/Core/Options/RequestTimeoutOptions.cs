using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures the maximum time allowed for processing a request.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record RequestTimeoutOptions
{
    /// <summary>
    /// Gets the request timeout, in seconds. Defaults to 100.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 100;
}
