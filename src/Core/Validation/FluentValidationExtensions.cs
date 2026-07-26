using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Humanizer;

namespace Aurore.Foundation.Core.Validation;

/// <summary>
/// Provides extension methods for converting FluentValidation results into Aurore's standard error shape.
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Converts a sequence of FluentValidation <see cref="ValidationFailure"/> instances into a normalized error
    /// dictionary, grouping messages by property location and formatting each location according to
    /// <see cref="ValidationErrorOptions.LocationFormat"/>.
    /// </summary>
    /// <param name="errors">The validation failures to normalize.</param>
    /// <returns>
    /// A dictionary with a single <c>"errors"</c> key containing one <see cref="ValidationErrorEntry"/> per distinct property location.
    /// </returns>
    public static Dictionary<string, object?> NormalizeErrors(this IEnumerable<ValidationFailure> errors)
    {
        return new()
        {
            ["errors"] = errors
                .Select(x => (Location: Locate(x.PropertyName), x.ErrorMessage))
                .GroupBy(x => x.Location)
                .Select(g => ToEntry(g.Key, [.. g.Select(x => x.ErrorMessage)]))
        };

        static (string Path, string Field) Locate(string propertyName)
        {
            var segments = propertyName
                .Replace('[', '/')
                .Replace("]", string.Empty)
                .Replace('.', '/')
                .Split('/')
                .Select(s => s.Camelize())
                .ToArray();

            return ($"/{string.Join('/', segments)}", string.Join('.', segments));
        }

        static ValidationErrorEntry ToEntry((string Path, string Field) location, string[] messages)
        {
            var format = ValidationErrorOptions.LocationFormat;

            return new ValidationErrorEntry(
                Path: format is ValidationErrorLocationFormat.PathOnly or ValidationErrorLocationFormat.All ? location.Path : null,
                Field: format is ValidationErrorLocationFormat.FieldOnly or ValidationErrorLocationFormat.All ? location.Field : null,
                Messages: messages);
        }
    }
}
