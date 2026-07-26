namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures idempotency key handling for incoming requests.
/// </summary>
public sealed record IdempotencyOptions
{
    /// <summary>
    /// Gets the number of minutes an idempotency key's cached result is retained. Defaults to 1440 (24 hours).
    /// </summary>
    public int CacheDurationMinutes { get; init; } = 1440;
}
