using System;
using System.Collections.Generic;

namespace Aurore.Foundation.Core.Security;

/// <summary>
/// Configures the <see cref="Obfuscator"/> during its one-time <see cref="Obfuscator.Configure"/> call, including
/// the encoding alphabet, minimum length, blocked words, and per-type salt registrations.
/// </summary>
public sealed class ObfuscatorRegistrationOptions
{
    private int _nextId = 1;

    internal Dictionary<Type, int> Types { get; } = [];

    /// <summary>
    /// Gets or sets the minimum length of an encoded identifier.
    /// </summary>
    public int MinimumLength { get; set; }

    /// <summary>
    /// Gets or sets the set of characters used to build encoded identifiers.
    /// </summary>
    public string Alphabet { get; set; } = "";

    /// <summary>
    /// Gets or sets the set of words that must never appear in an encoded identifier.
    /// </summary>
    public HashSet<string> BlockList { get; set; } = [];

    /// <summary>
    /// Registers <typeparamref name="T"/> with the next available type ID, used to salt encoded identifiers for that type.
    /// </summary>
    /// <typeparam name="T">The entity type to register.</typeparam>
    /// <returns>This instance, to allow chaining further registrations.</returns>
    public ObfuscatorRegistrationOptions Register<T>()
    {
        Types.Add(typeof(T), _nextId++);

        return this;
    }
}
