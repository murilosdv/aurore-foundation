using Aurore.Foundation.Core.Extensions;

namespace Tests.Core.Extensions;

public class ArrayExtensionsTests
{
    [Fact(DisplayName = "Join uses a single space as the default separator")]
    public void JoinUsesSpaceAsDefaultSeparator()
    {
        // Arrange
        var array = new[] { "Aurore", "Foundation", "Core" };

        // Act
        var result = array.Join();

        // Assert
        Assert.Equal("Aurore Foundation Core", result);
    }

    [Fact(DisplayName = "Join uses the provided separator when one is given")]
    public void JoinUsesProvidedSeparator()
    {
        // Arrange
        var array = new[] { "a", "b", "c" };

        // Act
        var result = array.Join(',');

        // Assert
        Assert.Equal("a,b,c", result);
    }
}
