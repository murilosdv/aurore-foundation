using System;
using Microsoft.Extensions.Options;

namespace Tests.Shared.Fakes;

/// <summary>
/// A minimal <see cref="IOptionsMonitor{TOptions}"/> whose <see cref="CurrentValue"/> is fixed at construction,
/// for tests that only need a constant options value and never call <see cref="Get"/> or <see cref="OnChange"/>.
/// </summary>
/// <typeparam name="TOptions">The options type being monitored.</typeparam>
/// <param name="value">The fixed value to expose as <see cref="CurrentValue"/>.</param>
public sealed class FakeOptionsMonitor<TOptions>(TOptions value) : IOptionsMonitor<TOptions>
{
    /// <inheritdoc/>
    public TOptions CurrentValue { get; } = value;

    /// <inheritdoc/>
    public TOptions Get(string? name)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public IDisposable OnChange(Action<TOptions, string?> listener)
    {
        throw new NotSupportedException();
    }
}
