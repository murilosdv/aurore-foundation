using System.Text.Json;

namespace Aurore.Foundation.CliCore.Extensions;

/// <summary>
/// Provides extension methods for <see cref="JsonElement"/>.
/// </summary>
public static class JsonElementExtensions
{
    /// <summary>
    /// Reads a string-valued property, returning <see langword="null"/> instead of throwing when
    /// the property is missing or isn't a string (or JSON null).
    /// </summary>
    /// <param name="element">The JSON object element to read from.</param>
    /// <param name="propertyName">The name of the property to read.</param>
    /// <returns>The property's string value, or <see langword="null"/> if it's missing, JSON null, or not a string.</returns>
    public static string? GetStringProperty(this JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
            return default;

        return property.ValueKind is JsonValueKind.String or JsonValueKind.Null
            ? property.GetString()
            : default;
    }
}
