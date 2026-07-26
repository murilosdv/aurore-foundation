using System;
using Aurore.Foundation.Core.Extensions;

namespace Tests.Core.Extensions;

public class EnumExtensionsTests
{
    private enum Color
    {
        Red,
        Green,
        Blue
    }

    [Fact(DisplayName = "ParseToEnum parses a matching value using a case-sensitive match")]
    public void ParseToEnumParsesExactMatch()
    {
        // Arrange
        var value = "Green";

        // Act
        var result = value.ParseToEnum<Color>();

        // Assert
        Assert.Equal(Color.Green, result);
    }

    [Fact(DisplayName = "ParseToEnum falls back to the default value when the case does not match")]
    public void ParseToEnumFallsBackOnCaseMismatch()
    {
        // Arrange
        var value = "green";

        // Act
        var result = value.ParseToEnum<Color>(Color.Blue);

        // Assert
        Assert.Equal(Color.Blue, result);
    }

    [Fact(DisplayName = "ParseToEnum falls back to the enum's default value when no fallback is supplied")]
    public void ParseToEnumFallsBackToEnumDefault()
    {
        // Arrange
        var value = "not-a-color";

        // Act
        var result = value.ParseToEnum<Color>();

        // Assert
        Assert.Equal(Color.Red, result);
    }

    [Fact(DisplayName = "TryParseEnum ignores case")]
    public void TryParseEnumIgnoresCase()
    {
        // Arrange
        var value = "green";

        // Act
        var result = value.TryParseEnum<Color>();

        // Assert
        Assert.Equal(Color.Green, result);
    }

    [Fact(DisplayName = "TryParseEnum returns the fallback value when parsing fails")]
    public void TryParseEnumReturnsFallbackOnFailure()
    {
        // Arrange
        var value = "not-a-color";

        // Act
        var result = value.TryParseEnum<Color>(Color.Blue);

        // Assert
        Assert.Equal(Color.Blue, result);
    }

    [Fact(DisplayName = "TryParseEnum throws InvalidCastException when parsing fails and no fallback is supplied")]
    public void TryParseEnumThrowsWithoutFallback()
    {
        // Arrange
        var value = "not-a-color";

        // Act & Assert
        Assert.Throws<InvalidCastException>(() => value.TryParseEnum<Color>());
    }
}
