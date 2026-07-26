using System.Text.Json.Serialization;

namespace Aurore.Foundation.Core.Validation;

/// <summary>
/// Represents the validation error messages associated with a single property location.
/// </summary>
/// <param name="Path">The JSON Pointer-style path to the invalid property (e.g. <c>/address/street</c>), or <see langword="null"/> if omitted by the configured <see cref="ValidationErrorLocationFormat"/>.</param>
/// <param name="Field">The dotted field name of the invalid property (e.g. <c>address.street</c>), or <see langword="null"/> if omitted by the configured <see cref="ValidationErrorLocationFormat"/>.</param>
/// <param name="Messages">The validation error messages for this property location.</param>
public sealed record ValidationErrorEntry(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Path,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Field,
    string[] Messages);
