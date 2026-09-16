using System;
using System.Collections.Generic;

namespace Aurore.Foundation.Core.Security;

/// <summary>
/// Registers the entity types <see cref="Obfuscator"/> can salt-encode, returned by its one-time
/// <see cref="Obfuscator.Configure"/> call. Each registration gets the next available type ID, in the order
/// <see cref="Register{T}"/> is called, which is why registration happens once at startup in a fixed order
/// rather than lazily on first use — an order that could vary between requests or instances would make the
/// same (type, id) pair encode to different strings depending on which one happened to run first.
/// </summary>
public sealed class ObfuscatorRegistrationOptions
{
    private int _nextId = 1;

    internal Dictionary<Type, int> Types { get; } = [];

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
