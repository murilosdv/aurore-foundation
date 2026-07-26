using System.Collections.Generic;
using System.Linq;
using Aurore.Foundation.Core.Validation;
using FluentValidation.Results;

namespace Tests.Core.Validation;

public class FluentValidationExtensionsTests
{
    [Fact(DisplayName = "NormalizeErrors camelizes each segment of a dotted property path")]
    public void NormalizeErrorsCamelizesDottedPropertyPath()
    {
        // Arrange
        var failures = new[] { new ValidationFailure("Address.Street", "Street is required.") };

        // Act
        var normalized = failures.NormalizeErrors();
        var entry = ((IEnumerable<ValidationErrorEntry>)normalized["errors"]!).Single();

        // Assert
        Assert.Equal("/address/street", entry.Path);
        Assert.Equal("address.street", entry.Field);
        Assert.Equal(["Street is required."], entry.Messages);
    }

    [Fact(DisplayName = "NormalizeErrors converts bracketed indexers into path segments")]
    public void NormalizeErrorsConvertsIndexersToPathSegments()
    {
        // Arrange
        var failures = new[] { new ValidationFailure("Items[0].Name", "Name is required.") };

        // Act
        var normalized = failures.NormalizeErrors();
        var entry = ((IEnumerable<ValidationErrorEntry>)normalized["errors"]!).Single();

        // Assert
        Assert.Equal("/items/0/name", entry.Path);
        Assert.Equal("items.0.name", entry.Field);
    }

    [Fact(DisplayName = "NormalizeErrors groups multiple messages for the same property under a single entry")]
    public void NormalizeErrorsGroupsMessagesByLocation()
    {
        // Arrange
        var failures = new[]
        {
            new ValidationFailure("Email", "Email is required."),
            new ValidationFailure("Email", "Email is not a valid address.")
        };

        // Act
        var normalized = failures.NormalizeErrors();
        var entries = ((IEnumerable<ValidationErrorEntry>)normalized["errors"]!).ToArray();

        // Assert
        var entry = Assert.Single(entries);
        Assert.Equal(["Email is required.", "Email is not a valid address."], entry.Messages);
    }

    [Fact(DisplayName = "NormalizeErrors omits Field when the location format is PathOnly")]
    public void NormalizeErrorsOmitsFieldForPathOnlyFormat()
    {
        // Arrange
        var original = ValidationErrorOptions.LocationFormat;
        ValidationErrorOptions.LocationFormat = ValidationErrorLocationFormat.PathOnly;

        try
        {
            var failures = new[] { new ValidationFailure("Email", "Email is required.") };

            // Act
            var normalized = failures.NormalizeErrors();
            var entry = ((IEnumerable<ValidationErrorEntry>)normalized["errors"]!).Single();

            // Assert
            Assert.Equal("/email", entry.Path);
            Assert.Null(entry.Field);
        }
        finally
        {
            ValidationErrorOptions.LocationFormat = original;
        }
    }

    [Fact(DisplayName = "NormalizeErrors omits Path when the location format is FieldOnly")]
    public void NormalizeErrorsOmitsPathForFieldOnlyFormat()
    {
        // Arrange
        var original = ValidationErrorOptions.LocationFormat;
        ValidationErrorOptions.LocationFormat = ValidationErrorLocationFormat.FieldOnly;

        try
        {
            var failures = new[] { new ValidationFailure("Email", "Email is required.") };

            // Act
            var normalized = failures.NormalizeErrors();
            var entry = ((IEnumerable<ValidationErrorEntry>)normalized["errors"]!).Single();

            // Assert
            Assert.Null(entry.Path);
            Assert.Equal("email", entry.Field);
        }
        finally
        {
            ValidationErrorOptions.LocationFormat = original;
        }
    }
}
