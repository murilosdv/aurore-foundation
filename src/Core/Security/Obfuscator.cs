using System;
using System.Collections.Generic;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Extensions;

namespace Aurore.Foundation.Core.Security;

/// <summary>
/// Provides methods to obfuscate integer identifiers into opaque strings (and back), optionally salting the
/// encoding per-type so the same numeric ID produces different strings for different entity types. Both
/// <see cref="int"/> and <see cref="long"/> identifiers are supported; the <see cref="long"/> overloads are
/// named distinctly (<c>DecodeLong</c>, <c>TryDecodeLong</c>, <c>DecodeLongOrThrow</c>) on the decode side
/// since a string can't be decoded back into two different return types via plain overloading.
/// </summary>
/// <remarks>
/// This is obfuscation for readability, not security — the scrambling (a fixed-point multiply-add, invertible
/// modulo 2^32/2^64) and the per-type salts are recoverable by anyone who has the alphabet and puts in the effort.
/// Don't use it to gate access to a resource; use it to keep sequential IDs out of URLs.
/// Every identifier of a given numeric type encodes to a fixed-width string regardless of its value — 6
/// characters for <see cref="int"/>, 11 for <see cref="long"/>, assuming the default 62-character alphabet
/// (fewer characters in the alphabet means more digits are needed to cover the same range). There's no
/// separate minimum-length setting: the width already covers that type's full value range, so nothing shorter
/// would be losslessly reversible in the first place.
/// Decoding with the wrong salt (e.g. <c>Decode&lt;Order&gt;()</c> on a string that was encoded for <c>Customer</c>,
/// or the salt-less <c>Decode()</c> on a salted string) is <b>not</b> detected — there's no redundant/checksum
/// information in the encoding to tell a correct decode from an incorrect one, so a mismatched salt just produces
/// a different, wrong number instead of failing. Callers need to know from context (e.g. which route/field a
/// value came from) which type it was encoded for; the salt only differentiates output between types, it doesn't
/// self-describe which type was used.
/// </remarks>
public static class Obfuscator
{
    private const uint Multiplier32 = 2654435761;
    private const ulong Multiplier64 = 0x9E3779B97F4A7C15;

    private static Dictionary<Type, int> _types = [];
    private static bool _configured;
    private static string _alphabet = DefaultValues.AlphaNumericAlphabet;

    /// <summary>
    /// Registers the entity types that can be salt-encoded, and optionally the alphabet used to render encoded
    /// identifiers. Can only be called once per application lifetime.
    /// </summary>
    /// <param name="alphabet">
    /// The set of characters used to build encoded identifiers. Defaults to <see cref="DefaultValues.AlphaNumericAlphabet"/>
    /// when not provided. A custom, shuffled alphabet makes encoded identifiers harder to recognize as coming from this scheme.
    /// </param>
    /// <returns>An <see cref="ObfuscatorRegistrationOptions"/> to register entity types on, via chained calls to <see cref="ObfuscatorRegistrationOptions.Register{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the obfuscator has already been configured.</exception>
    public static ObfuscatorRegistrationOptions Configure(string? alphabet = null)
    {
        if (_configured)
            throw new InvalidOperationException("Obfuscator has already been configured.");

        if (alphabet.HasValue())
            _alphabet = alphabet;

        var options = new ObfuscatorRegistrationOptions();

        _types = options.Types;

        _configured = true;

        return options;
    }

    /// <summary>
    /// Encodes an integer identifier into an obfuscated string.
    /// </summary>
    /// <param name="id">The identifier to encode.</param>
    /// <returns>The obfuscated string representation of <paramref name="id"/>.</returns>
    public static string Encode(this int id)
    {
        return EncodeCore((uint)id, 0);
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
        return EncodeCore((uint)id, (uint)GetTypeId<T>());
    }

    /// <summary>
    /// Decodes an obfuscated string back into its integer identifier.
    /// </summary>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier, or -1 if <paramref name="encoded"/> could not be decoded.</returns>
    public static int Decode(this string encoded)
    {
        return TryDecodeCore32(encoded, 0, out var id) ? (int)id : -1;
    }

    /// <summary>
    /// Decodes an obfuscated string that was encoded with a type salt back into its integer identifier.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier is expected to belong to.</typeparam>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier, or -1 if <paramref name="encoded"/> could not be decoded.</returns>
    public static int Decode<T>(this string encoded) where T : class
    {
        return TryDecodeCore32(encoded, (uint)GetTypeId<T>(), out var id) ? (int)id : -1;
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

    /// <summary>
    /// Encodes a long identifier into an obfuscated string.
    /// </summary>
    /// <param name="id">The identifier to encode.</param>
    /// <returns>The obfuscated string representation of <paramref name="id"/>.</returns>
    public static string Encode(this long id)
    {
        return EncodeCore((ulong)id, 0);
    }

    /// <summary>
    /// Encodes a long identifier into an obfuscated string, salted with the registered type ID for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier belongs to. Must have been registered via <see cref="ObfuscatorRegistrationOptions.Register{T}"/>.</typeparam>
    /// <param name="id">The identifier to encode.</param>
    /// <returns>The obfuscated string representation of <paramref name="id"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="T"/> has not been registered.</exception>
    public static string Encode<T>(this long id) where T : class
    {
        return EncodeCore((ulong)id, (ulong)GetTypeId<T>());
    }

    /// <summary>
    /// Decodes an obfuscated string back into its long identifier.
    /// </summary>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier, or -1 if <paramref name="encoded"/> could not be decoded.</returns>
    public static long DecodeLong(this string encoded)
    {
        return TryDecodeCore64(encoded, 0, out var id) ? (long)id : -1;
    }

    /// <summary>
    /// Decodes an obfuscated string that was encoded with a type salt back into its long identifier.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier is expected to belong to.</typeparam>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier, or -1 if <paramref name="encoded"/> could not be decoded.</returns>
    public static long DecodeLong<T>(this string encoded) where T : class
    {
        return TryDecodeCore64(encoded, (ulong)GetTypeId<T>(), out var id) ? (long)id : -1;
    }

    /// <summary>
    /// Attempts to decode an obfuscated string back into its long identifier.
    /// </summary>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <param name="id">When this method returns, contains the decoded identifier if successful, or -1 otherwise.</param>
    /// <returns><see langword="true"/> if decoding succeeded; otherwise, <see langword="false"/>.</returns>
    public static bool TryDecodeLong(this string encoded, out long id)
    {
        id = DecodeLong(encoded);

        return id >= 0;
    }

    /// <summary>
    /// Attempts to decode an obfuscated, type-salted string back into its long identifier.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier is expected to belong to.</typeparam>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <param name="id">When this method returns, contains the decoded identifier if successful, or -1 otherwise.</param>
    /// <returns><see langword="true"/> if decoding succeeded; otherwise, <see langword="false"/>.</returns>
    public static bool TryDecodeLong<T>(this string encoded, out long id) where T : class
    {
        id = DecodeLong<T>(encoded);

        return id >= 0;
    }

    /// <summary>
    /// Decodes an obfuscated string back into its long identifier, throwing if decoding fails.
    /// </summary>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="encoded"/> cannot be decoded.</exception>
    public static long DecodeLongOrThrow(this string encoded)
    {
        var id = DecodeLong(encoded);

        if (id >= 0)
            return id;

        throw new ArgumentException($"Cannot decode invalid obfuscated string: '{encoded}'", nameof(encoded));
    }

    /// <summary>
    /// Decodes an obfuscated, type-salted string back into its long identifier, throwing if decoding fails.
    /// </summary>
    /// <typeparam name="T">The entity type the identifier is expected to belong to.</typeparam>
    /// <param name="encoded">The obfuscated string to decode.</param>
    /// <returns>The decoded identifier.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="encoded"/> cannot be decoded.</exception>
    public static long DecodeLongOrThrow<T>(this string encoded) where T : class
    {
        var id = DecodeLong<T>(encoded);

        if (id >= 0)
            return id;

        throw new ArgumentException($"Cannot decode invalid obfuscated string: '{encoded}'", nameof(encoded));
    }

    private static string EncodeCore(uint value, uint salt)
    {
        var scrambled = (value * Multiplier32) + salt;
        var digits = DigitsFor(32, _alphabet.Length);

        return EncodeDigits(scrambled, digits, _alphabet);
    }

    private static string EncodeCore(ulong value, ulong salt)
    {
        var scrambled = (value * Multiplier64) + salt;
        var digits = DigitsFor(64, _alphabet.Length);

        return EncodeDigits(scrambled, digits, _alphabet);
    }

    private static bool TryDecodeCore32(string encoded, uint salt, out uint value)
    {
        value = 0;

        var digits = DigitsFor(32, _alphabet.Length);

        if (encoded.Length != digits || DecodeDigits(encoded, _alphabet) is not { } scrambled || scrambled > uint.MaxValue)
            return false;

        value = ((uint)scrambled - salt) * ModularInverse(Multiplier32);

        return true;
    }

    private static bool TryDecodeCore64(string encoded, ulong salt, out ulong value)
    {
        value = 0;

        var digits = DigitsFor(64, _alphabet.Length);

        if (encoded.Length != digits || DecodeDigits(encoded, _alphabet) is not { } scrambled)
            return false;

        value = (scrambled - salt) * ModularInverse(Multiplier64);

        return true;
    }

    /// <summary>
    /// Computes how many digits, in an alphabet of <paramref name="alphabetLength"/> characters, are needed to
    /// losslessly represent every value in a <paramref name="bits"/>-wide unsigned integer.
    /// </summary>
    private static int DigitsFor(int bits, int alphabetLength)
    {
        return (int)Math.Ceiling(bits / Math.Log2(alphabetLength));
    }

    private static string EncodeDigits(ulong value, int digitCount, string alphabet)
    {
        var chars = new char[digitCount];
        var radix = (ulong)alphabet.Length;

        for (var i = digitCount - 1; i >= 0; i--)
        {
            chars[i] = alphabet[(int)(value % radix)];
            value /= radix;
        }

        return new string(chars);
    }

    private static ulong? DecodeDigits(string encoded, string alphabet)
    {
        var radix = (ulong)alphabet.Length;
        var value = 0UL;

        foreach (var c in encoded)
        {
            var digit = alphabet.IndexOf(c);

            if (digit < 0)
                return null;

            value = (value * radix) + (ulong)digit;
        }

        return value;
    }

    /// <summary>
    /// Computes the multiplicative inverse of an odd number modulo 2^32, via Newton's iteration
    /// (each pass doubles the number of correct low bits: 1 -> 2 -> 4 -> ... -> 32).
    /// </summary>
    private static uint ModularInverse(uint value)
    {
        var inverse = value;

        for (var i = 0; i < 5; i++)
            inverse *= 2 - (value * inverse);

        return inverse;
    }

    /// <summary>
    /// Computes the multiplicative inverse of an odd number modulo 2^64, via Newton's iteration
    /// (each pass doubles the number of correct low bits: 1 -> 2 -> 4 -> ... -> 64).
    /// </summary>
    private static ulong ModularInverse(ulong value)
    {
        var inverse = value;

        for (var i = 0; i < 6; i++)
            inverse *= 2 - (value * inverse);

        return inverse;
    }

    private static int GetTypeId<T>()
    {
        var type = typeof(T);

        if (_types.TryGetValue(type, out var id))
            return id;

        throw new InvalidOperationException($"Type '{type.Name}' has not been registered.");
    }
}
