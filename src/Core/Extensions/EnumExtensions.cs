using System;
using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Extensions;

/// <summary>
/// Provides extension methods for parsing strings into enum values.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Attempts to parse <paramref name="value"/> as a <typeparamref name="TEnum"/> value using a case-sensitive match,
    /// falling back to <paramref name="defaultValue"/> (or the enum's default value) when parsing fails.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to parse into.</typeparam>
    /// <param name="value">The string to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to the enum's default value.</param>
    /// <returns>The parsed enum value, or the fallback value if parsing fails.</returns>
    public static TEnum ParseToEnum<TEnum>(this string value, TEnum? defaultValue = null)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, out var result))
            return result;

        return defaultValue ?? default;
    }

    /// <summary>
    /// Attempts to parse <paramref name="value"/> as a <typeparamref name="T"/> value using a case-insensitive match.
    /// </summary>
    /// <typeparam name="T">The enum type to parse into.</typeparam>
    /// <param name="value">The string to parse.</param>
    /// <param name="fallbackValue">The value to return if parsing fails. If not supplied, an exception is thrown instead.</param>
    /// <returns>The parsed enum value, or <paramref name="fallbackValue"/> if parsing fails and a fallback was supplied.</returns>
    /// <exception cref="InvalidCastException">Thrown when <paramref name="value"/> cannot be parsed and no <paramref name="fallbackValue"/> was supplied.</exception>
    public static T TryParseEnum<T>([NotNullWhen(false)] this string? value, T? fallbackValue = null) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out var result))
            return result;

        return fallbackValue ?? throw new InvalidCastException($"'{value}' is not a valid '{typeof(T).Name}' enum value.");
    }
}
