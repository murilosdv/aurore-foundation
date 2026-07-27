using Microsoft.AspNetCore.Http;

namespace Aurore.Foundation.Tests.AspNetCore.MinimalApis;

public class ResourceGroupTests
{
    [Fact(DisplayName = "TypeName defaults to the class name with the Resource suffix removed")]
    public void TypeNameDefaultsToClassNameWithoutResourceSuffix()
    {
        // Arrange
        var group = new WidgetsResource();

        // Act
        var typeName = group.TypeName;

        // Assert
        Assert.Equal("Widgets", typeName);
    }

    [Fact(DisplayName = "Name defaults to TypeName in kebab-case")]
    public void NameDefaultsToKebabCasedTypeName()
    {
        // Arrange
        var group = new WidgetsResource();

        // Act
        var name = group.Name;

        // Assert
        Assert.Equal("widgets", name);
    }

    [Fact(DisplayName = "Tags defaults to TypeName in Pascal case")]
    public void TagsDefaultsToPascalCasedTypeName()
    {
        // Arrange
        var group = new WidgetsResource();

        // Act
        var tags = group.Tags;

        // Assert
        Assert.Equal(["Widgets"], tags);
    }

    [Fact(DisplayName = "PrefixWithApi defaults to false")]
    public void PrefixWithApiDefaultsToFalse()
    {
        // Arrange
        var group = new WidgetsResource();

        // Act
        var prefixWithApi = group.PrefixWithApi;

        // Assert
        Assert.False(prefixWithApi);
    }

    [Fact(DisplayName = "RequireAuthorization defaults to true")]
    public void RequireAuthorizationDefaultsToTrue()
    {
        // Arrange
        var group = new WidgetsResource();

        // Act
        var requireAuthorization = group.RequireAuthorization;

        // Assert
        Assert.True(requireAuthorization);
    }

    [Fact(DisplayName = "Versions defaults to [1]")]
    public void VersionsDefaultsToOne()
    {
        // Arrange
        var group = new WidgetsResource();

        // Act
        var versions = group.Versions;

        // Assert
        Assert.Equal([1], versions);
    }

    [Fact(DisplayName = "Problems defaults to [500]")]
    public void ProblemsDefaultsToInternalServerError()
    {
        // Arrange
        var group = new WidgetsResource();

        // Act
        var problems = group.Problems;

        // Assert
        Assert.Equal([StatusCodes.Status500InternalServerError], problems);
    }
}
