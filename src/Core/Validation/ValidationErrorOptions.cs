namespace Aurore.Foundation.Core.Validation;

/// <summary>
/// Holds process-wide configuration controlling how <see cref="FluentValidationExtensions.NormalizeErrors"/> formats error locations.
/// </summary>
public static class ValidationErrorOptions
{
    /// <summary>
    /// Gets or sets the location format applied when normalizing validation errors. Defaults to <see cref="ValidationErrorLocationFormat.All"/>.
    /// </summary>
    public static ValidationErrorLocationFormat LocationFormat { get; set; } = ValidationErrorLocationFormat.All;
}
