using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures whether the application is operating in maintenance mode and how it communicates that to callers.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record MaintenanceModeOptions
{
    /// <summary>
    /// Gets a value indicating whether maintenance mode is currently active.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// Gets the message returned to callers while maintenance mode is active, or <see langword="null"/> if none is configured.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the number of seconds callers are advised to wait before retrying, via the <c>Retry-After</c> header. Defaults to 60.
    /// </summary>
    public int RetryAfterSeconds { get; init; } = 60;
}
