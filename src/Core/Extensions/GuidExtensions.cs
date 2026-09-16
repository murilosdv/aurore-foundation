using System;

namespace Aurore.Foundation.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Guid"/>.
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    /// Shortens the identifier into a compact, URL-safe, base64-encoded string (22 characters, no padding),
    /// reversible via <see cref="Unshorten"/> or <see cref="TryUnshorten"/>. This re-encodes the same 128 bits,
    /// it does not hash the identifier, so the original value can always be recovered from the result.
    /// </summary>
    /// <param name="id">The identifier to shorten.</param>
    /// <returns>The shortened, URL-safe string representation of <paramref name="id"/>.</returns>
    public static string Shorten(this Guid id)
    {
        Span<byte> bytes = stackalloc byte[16];
        id.TryWriteBytes(bytes);

        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    /// <summary>
    /// Expands a string produced by <see cref="Shorten"/> back into the original identifier.
    /// </summary>
    /// <param name="shortened">The shortened string to expand.</param>
    /// <returns>The original <see cref="Guid"/>.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="shortened"/> is not a valid shortened identifier.</exception>
    public static Guid Unshorten(this string shortened)
    {
        return shortened.TryUnshorten(out var id)
            ? id
            : throw new FormatException($"Cannot expand invalid shortened identifier: '{shortened}'");
    }

    /// <summary>
    /// Attempts to expand a string produced by <see cref="Shorten"/> back into the original identifier.
    /// </summary>
    /// <param name="shortened">The shortened string to expand.</param>
    /// <param name="id">When this method returns, contains the expanded identifier if successful, or <see cref="Guid.Empty"/> otherwise.</param>
    /// <returns><see langword="true"/> if expansion succeeded; otherwise, <see langword="false"/>.</returns>
    public static bool TryUnshorten(this string shortened, out Guid id)
    {
        id = Guid.Empty;

        var padded = shortened.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + ((4 - (padded.Length % 4)) % 4), '=');

        Span<byte> bytes = stackalloc byte[16];

        if (Convert.TryFromBase64String(padded, bytes, out var written) is false || written != 16)
            return false;

        id = new Guid(bytes);

        return true;
    }
}
