using System;
using Aurore.Foundation.Core.Extensions;

namespace Tests.Core.Extensions;

public class StringExtensionsTests
{
    [Fact(DisplayName = "HasValue returns false for null, empty or whitespace strings")]
    public void HasValueReturnsFalseForNullOrBlank()
    {
        // Arrange
        string? nullValue = null;

        // Act
        var nullResult = nullValue.HasValue();
        var emptyResult = "".HasValue();
        var whitespaceResult = "   ".HasValue();

        // Assert
        Assert.False(nullResult);
        Assert.False(emptyResult);
        Assert.False(whitespaceResult);
    }

    [Fact(DisplayName = "HasValue returns true for a non-empty string")]
    public void HasValueReturnsTrueForNonEmptyString()
    {
        // Arrange
        var value = "aurore";

        // Act
        var result = value.HasValue();

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "IsEqualTo ignores case and culture by default")]
    public void IsEqualToIgnoresCaseByDefault()
    {
        // Arrange
        var left = "AURORE";
        var right = "aurore";

        // Act
        var result = left.IsEqualTo(right);

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "IsEqualTo returns false for genuinely different strings")]
    public void IsEqualToReturnsFalseForDifferentStrings()
    {
        // Arrange
        var left = "aurore";
        var right = "foundation";

        // Act
        var result = left.IsEqualTo(right);

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "HasUniqueCharacters returns true only when no character repeats")]
    public void HasUniqueCharactersDetectsRepeats()
    {
        // Arrange
        var unique = "abcdef";
        var repeated = "aurore";

        // Act
        var uniqueResult = unique.HasUniqueCharacters();
        var repeatedResult = repeated.HasUniqueCharacters();

        // Assert
        Assert.True(uniqueResult);
        Assert.False(repeatedResult);
    }

    [Fact(DisplayName = "ToCacheKey joins the name and keys with a colon and kebab-cases the result")]
    public void ToCacheKeyBuildsKebabCasedKey()
    {
        // Arrange
        var name = "UserProfile";

        // Act
        var result = name.ToCacheKey("42", "OrderHistory");

        // Assert
        Assert.Equal("user-profile:42:order-history", result);
    }

    [Fact(DisplayName = "ToDateTimeOffset parses a recognizable date/time string")]
    public void ToDateTimeOffsetParsesValidInput()
    {
        // Arrange
        var input = "2026-07-25T10:00:00+00:00";

        // Act
        var result = input.ToDateTimeOffset();

        // Assert
        Assert.Equal(new DateTimeOffset(2026, 7, 25, 10, 0, 0, TimeSpan.Zero), result);
    }

    [Fact(DisplayName = "ToDateTimeOffset throws FormatException for an unrecognizable string")]
    public void ToDateTimeOffsetThrowsForInvalidInput()
    {
        // Arrange
        var input = "not-a-date";

        // Act & Assert
        Assert.Throws<FormatException>(() => input.ToDateTimeOffset());
    }

    [Fact(DisplayName = "RemoveDiacritics strips accents and maps special Latin characters to their closest equivalent")]
    public void RemoveDiacriticsStripsAccentsAndSpecialCharacters()
    {
        // Arrange
        var value = "Åse Øst þis ł";

        // Act
        var result = value.RemoveDiacritics();

        // Assert
        Assert.Equal("Ase Ost tis l", result);
    }

    [Fact(DisplayName = "RemoveDiacritics leaves plain ASCII strings unchanged")]
    public void RemoveDiacriticsLeavesPlainAsciiUnchanged()
    {
        // Arrange
        var value = "Aurore Foundation";

        // Act
        var result = value.RemoveDiacritics();

        // Assert
        Assert.Equal(value, result);
    }
}
