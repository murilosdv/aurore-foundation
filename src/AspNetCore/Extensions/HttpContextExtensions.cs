using Microsoft.AspNetCore.Http;

namespace Aurore.Foundation.AspNetCore.Extensions;

/// <summary>
/// Provides extension methods for reading data from an <see cref="HttpContext"/>.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the value of the given request header, or a fallback value when the header is absent.
    /// </summary>
    /// <param name="context">The HTTP context to read the header from.</param>
    /// <param name="key">The name of the header to read.</param>
    /// <param name="fallbackValue">The value to return when the header is not present. Defaults to <see langword="null"/>, which results in <see cref="string.Empty"/>.</param>
    /// <returns>The header value, or <paramref name="fallbackValue"/> (or an empty string) if the header is not present.</returns>
    public static string GetRequestHeader(this HttpContext context, string key, string? fallbackValue = null)
    {
        return context.Request.Headers.TryGetValue(key, out var headerValue) is not true
            ? fallbackValue ?? string.Empty
            : headerValue.ToString();
    }
}
