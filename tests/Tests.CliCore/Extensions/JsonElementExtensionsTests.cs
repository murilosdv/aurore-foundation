using System.Text.Json;
using Aurore.Foundation.CliCore.Extensions;

namespace Aurore.Foundation.Tests.CliCore.Extensions;

public class JsonElementExtensionsTests
{
    [Fact(DisplayName = "GetStringProperty returns the value when the property is a string")]
    public void GetStringPropertyReturnsValueWhenPresent()
    {
        // Arrange
        var element = JsonDocument.Parse("""{"name":"aurore"}""").RootElement;

        // Act
        var result = element.GetStringProperty("name");

        // Assert
        Assert.Equal("aurore", result);
    }

    [Fact(DisplayName = "GetStringProperty returns null when the property doesn't exist")]
    public void GetStringPropertyReturnsNullWhenMissing()
    {
        // Arrange
        var element = JsonDocument.Parse("{}").RootElement;

        // Act
        var result = element.GetStringProperty("name");

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "GetStringProperty returns null instead of throwing when the property isn't a string")]
    public void GetStringPropertyReturnsNullForNonStringValue()
    {
        // Arrange
        var element = JsonDocument.Parse("""{"count":42}""").RootElement;

        // Act
        var result = element.GetStringProperty("count");

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "GetStringProperty returns null when the property is explicitly JSON null")]
    public void GetStringPropertyReturnsNullForJsonNull()
    {
        // Arrange
        var element = JsonDocument.Parse("""{"name":null}""").RootElement;

        // Act
        var result = element.GetStringProperty("name");

        // Assert
        Assert.Null(result);
    }
}
