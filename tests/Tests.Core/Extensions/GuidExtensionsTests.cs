using System;
using Aurore.Foundation.Core.Extensions;

namespace Aurore.Foundation.Tests.Core.Extensions;

public class GuidExtensionsTests
{
    [Fact(DisplayName = "Shorten then Unshorten round-trips back to the original identifier")]
    public void ShortenThenUnshortenRoundTrips()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var shortened = id.Shorten();
        var expanded = shortened.Unshorten();

        // Assert
        Assert.Equal(id, expanded);
    }

    [Fact(DisplayName = "Shorten produces a 22-character, URL-safe string with no padding")]
    public void ShortenProducesUrlSafeStringWithoutPadding()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var shortened = id.Shorten();

        // Assert
        Assert.Equal(22, shortened.Length);
        Assert.DoesNotContain('+', shortened);
        Assert.DoesNotContain('/', shortened);
        Assert.DoesNotContain('=', shortened);
    }

    [Fact(DisplayName = "Shorten then Unshorten round-trips for the empty identifier")]
    public void ShortenThenUnshortenRoundTripsForEmptyGuid()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var shortened = id.Shorten();
        var expanded = shortened.Unshorten();

        // Assert
        Assert.Equal(id, expanded);
    }

    [Fact(DisplayName = "TryUnshorten returns true and the original identifier for a validly shortened string")]
    public void TryUnshortenReturnsTrueForValidInput()
    {
        // Arrange
        var id = Guid.NewGuid();
        var shortened = id.Shorten();

        // Act
        var succeeded = shortened.TryUnshorten(out var expanded);

        // Assert
        Assert.True(succeeded);
        Assert.Equal(id, expanded);
    }

    [Fact(DisplayName = "TryUnshorten returns false and Guid.Empty for a string that is not validly shortened")]
    public void TryUnshortenReturnsFalseForInvalidInput()
    {
        // Arrange
        var invalid = "not-a-valid-shortened-guid!";

        // Act
        var succeeded = invalid.TryUnshorten(out var id);

        // Assert
        Assert.False(succeeded);
        Assert.Equal(Guid.Empty, id);
    }

    [Fact(DisplayName = "TryUnshorten returns false for a string that decodes to the wrong byte length")]
    public void TryUnshortenReturnsFalseForWrongByteLength()
    {
        // Arrange
        var tooShort = Convert.ToBase64String([1, 2, 3]).TrimEnd('=');

        // Act
        var succeeded = tooShort.TryUnshorten(out var id);

        // Assert
        Assert.False(succeeded);
        Assert.Equal(Guid.Empty, id);
    }

    [Fact(DisplayName = "Unshorten throws FormatException for a string that is not validly shortened")]
    public void UnshortenThrowsForInvalidInput()
    {
        // Arrange
        var invalid = "not-a-valid-shortened-guid!";

        // Act & Assert
        Assert.Throws<FormatException>(() => invalid.Unshorten());
    }
}
