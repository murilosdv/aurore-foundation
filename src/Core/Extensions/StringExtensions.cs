using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Humanizer;

namespace Aurore.Foundation.Core.Extensions;

/// <summary>
/// Provides extension methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Determines whether the string is not <see langword="null"/>, empty, or composed entirely of whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns><see langword="true"/> if the string has a meaningful value; otherwise, <see langword="false"/>.</returns>
    public static bool HasValue([NotNullWhen(true)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value) is false;
    }

    /// <summary>
    /// Determines whether two strings are equal using a culture-invariant, case-insensitive comparison.
    /// </summary>
    /// <param name="value">The string to compare.</param>
    /// <param name="target">The string to compare against.</param>
    /// <returns><see langword="true"/> if the strings are equal, ignoring case; otherwise, <see langword="false"/>.</returns>
    public static bool IsEqualTo([NotNullWhen(false)] this string value, string target)
    {
        return string.Equals(value, target, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Determines whether every character in the string is unique (no character repeats).
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns><see langword="true"/> if all characters are unique; otherwise, <see langword="false"/>.</returns>
    public static bool HasUniqueCharacters(this string input)
    {
        return input.All(new HashSet<char>().Add);
    }

    /// <summary>
    /// Builds a kebab-cased cache key from a base name and any number of additional key segments, joined with <c>:</c>.
    /// </summary>
    /// <param name="name">The base name for the cache key.</param>
    /// <param name="keys">Additional segments to append to the cache key.</param>
    /// <returns>The resulting kebab-cased cache key.</returns>
    public static string ToCacheKey(this string name, params string[] keys)
    {
        return string.Join(':', [name, .. keys]).Kebaberize();
    }

    /// <summary>
    /// Parses the string as a <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <returns>The parsed <see cref="DateTimeOffset"/>.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="input"/> is not a recognizable date/time value.</exception>
    public static DateTimeOffset ToDateTimeOffset(this string input)
    {
        return DateTimeOffset.TryParse(input, out var datetime)
            ? datetime
            : throw new FormatException("Input string was not recognized as a DateTime");
    }

    /// <summary>
    /// Removes diacritical marks (accents) from the string, mapping special Latin characters
    /// (e.g. <c>å</c>, <c>ø</c>, <c>þ</c>, <c>ł</c>) to their closest unaccented equivalents.
    /// </summary>
    /// <param name="value">The string to strip diacritics from.</param>
    /// <returns>The string with diacritics removed.</returns>
    public static string RemoveDiacritics(this string value)
    {
        var sb = new StringBuilder();

        foreach (var c in value.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                var replacement = c switch
                {
                    'Å' or 'å' => char.IsUpper(c) ? 'A' : 'a',
                    'Æ' or 'æ' => c,
                    'Ð' => 'D',
                    'ð' => 'd',
                    'Ø' => 'O',
                    'ø' => 'o',
                    'Þ' => 'T',
                    'þ' => 't',
                    'Ł' => 'L',
                    'ł' => 'l',
                    _ => c
                };

                sb.Append(replacement);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
