using System;
using System.Collections.Generic;
using System.Threading;
using Aurore.Foundation.Core.Constants;
using Sqids;

namespace Aurore.Foundation.Core.Security;

/// <summary>
/// Provides methods to obfuscate integer identifiers into opaque, URL-safe strings (and back), optionally
/// salting the encoding per-type so the same numeric ID produces different strings for different entity types.
/// </summary>
public static class Obfuscator
{
    private static readonly Lock _lock = new();

    private static Dictionary<Type, int> _types = [];
    private static bool _configured = false;

    /// <summary>
    /// Gets the underlying <see cref="SqidsEncoder{T}"/> used to encode and decode identifiers. If <see cref="Configure"/>
    /// has not been called, a default instance (minimum length 5, alphanumeric alphabet) is lazily created on first access.
    /// </summary>
    public static SqidsEncoder<int> Instance
    {
        get
        {
            if (field is not null)
                return field;

            lock (_lock)
            {
                field ??= new(new()
                {
                    MinLength = 5,
                    Alphabet = DefaultValues.AlphaNumericAlphabet
                });
            }

            return field;
        }

        private set;
    }

    /// <summary>
    /// Configures the obfuscation alphabet, minimum length, block list, and per-type registrations used by <see cref="Instance"/>.
    /// Can only be called once per application lifetime.
    /// </summary>
    /// <param name="configure">A callback used to populate an <see cref="ObfuscatorRegistrationOptions"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown if the obfuscator has already been configured.</exception>
    public static void Configure(Action<ObfuscatorRegistrationOptions> configure)
    {
        if (_configured)
            throw new InvalidOperationException("Obfuscator has already been configured.");

        var options = new ObfuscatorRegistrationOptions();

        configure.Invoke(options);

        Instance = new(new()
        {
            MinLength = options.MinimumLength,
            Alphabet = options.Alphabet,
            BlockList = options.BlockList
        });

        _types = options.Types;

        _configured = true;
    }

    /// <summary>
    /// Encodes an integer identifier into an obfuscated string.
    /// </summary>
    /// <param name="id">The identifier to encode.</param>
    /// <returns>The obfuscated string representation of <paramref name="id"/>.</returns>
    public static string Encode(this int id)
    {
        return Instance.Encode(id);
    }

    /// <summary>
    /// Encodes an integer identifier into an obfuscated string, salted with the registered type ID for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier belongs to. Must have been registered via <see cref="ObfuscatorRegistrationOptions.Register{T}"/>.</typeparam>
    /// <param name="id">The identifier to encode.</param>
    /// <returns>The obfuscated string representation of <paramref name="id"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="T"/> has not been registered.</exception>
    public static string Encode<T>(this int id) where T : class
    {
        return Instance.Encode(GetTypeId<T>(), id);
    }

    /// <summary>
    /// Decodes an obfuscated string back into its integer identifier.
    /// </summary>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier, or -1 if <paramref name="encoded"/> could not be decoded to a single value.</returns>
    public static int Decode(this string encoded)
    {
        return Instance.Decode(encoded) is [var id] ? id : -1;
    }

    /// <summary>
    /// Decodes an obfuscated string that was encoded with a type salt back into its integer identifier.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier is expected to belong to.</typeparam>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier, or -1 if <paramref name="encoded"/> could not be decoded to a type-salted value.</returns>
    public static int Decode<T>(this string encoded) where T : class
    {
        return Instance.Decode(encoded) is [_, var id] ? id : -1;
    }

    /// <summary>
    /// Attempts to decode an obfuscated string back into its integer identifier.
    /// </summary>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <param name="id">When this method returns, contains the decoded identifier if successful, or -1 otherwise.</param>
    /// <returns><see langword="true"/> if decoding succeeded; otherwise, <see langword="false"/>.</returns>
    public static bool TryDecode(this string encoded, out int id)
    {
        id = Decode(encoded);

        return id >= 0;
    }

    /// <summary>
    /// Attempts to decode an obfuscated, type-salted string back into its integer identifier.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier is expected to belong to.</typeparam>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <param name="id">When this method returns, contains the decoded identifier if successful, or -1 otherwise.</param>
    /// <returns><see langword="true"/> if decoding succeeded; otherwise, <see langword="false"/>.</returns>
    public static bool TryDecode<T>(this string encoded, out int id) where T : class
    {
        id = Decode<T>(encoded);

        return id >= 0;
    }

    /// <summary>
    /// Decodes an obfuscated string back into its integer identifier, throwing if decoding fails.
    /// </summary>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="encoded"/> cannot be decoded.</exception>
    public static int DecodeOrThrow(this string encoded)
    {
        var id = Decode(encoded);

        if (id >= 0)
            return id;

        throw new ArgumentException($"Cannot decode invalid obfuscated string: '{encoded}'", nameof(encoded));
    }

    /// <summary>
    /// Decodes an obfuscated, type-salted string back into its integer identifier, throwing if decoding fails.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier is expected to belong to.</typeparam>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="encoded"/> cannot be decoded.</exception>
    public static int DecodeOrThrow<T>(this string encoded) where T : class
    {
        var id = Decode<T>(encoded);

        if (id >= 0)
            return id;

        throw new ArgumentException($"Cannot decode invalid obfuscated string: '{encoded}'", nameof(encoded));
    }

    private static int GetTypeId<T>()
    {
        var type = typeof(T);

        if (_types.TryGetValue(type, out var id))
            return id;

        throw new InvalidOperationException($"Type '{type.Name}' has not been registered.");
    }
}
