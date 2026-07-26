namespace Aurore.Foundation.Core.Validation;

/// <summary>
/// Determines which location representation(s) are included on a <see cref="ValidationErrorEntry"/>.
/// </summary>
public enum ValidationErrorLocationFormat
{
    /// <summary>Only <see cref="ValidationErrorEntry.Path"/> is populated.</summary>
    PathOnly,

    /// <summary>Only <see cref="ValidationErrorEntry.Field"/> is populated.</summary>
    FieldOnly,

    /// <summary>Both <see cref="ValidationErrorEntry.Path"/> and <see cref="ValidationErrorEntry.Field"/> are populated.</summary>
    All
}
