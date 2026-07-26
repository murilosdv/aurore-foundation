namespace Aurore.Foundation.Core.Extensions;

/// <summary>
/// Provides extension methods for arrays.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Concatenates the elements of a string array using the specified separator.
    /// </summary>
    /// <param name="array">The array of strings to join.</param>
    /// <param name="separator">The separator to insert between elements. Defaults to a single space when not specified.</param>
    /// <returns>The concatenated string.</returns>
    public static string Join(this string[] array, char? separator = null)
    {
        return string.Join(separator ?? ' ', array);
    }
}
