using Aurore.Foundation.Core.Errors;

namespace Aurore.Foundation.Tests.Core.Errors;

public class ApplicationErrorTests
{
    [Fact(DisplayName = "HttpStatusCode defaults to the standard status code for the error's category")]
    public void HttpStatusCodeDefaultsToCategoryStatusCode()
    {
        // Arrange
        var error = new ApplicationError(9001, "SomeError", "Some Error", "Something went wrong.", ErrorCategory.NotFound);

        // Act
        var statusCode = error.HttpStatusCode;

        // Assert
        Assert.Equal(404, statusCode);
    }

    [Fact(DisplayName = "HttpStatusCode keeps an explicitly assigned value instead of the category default")]
    public void HttpStatusCodeKeepsExplicitOverride()
    {
        // Arrange
        var error = new ApplicationError(9002, "Locked", "Locked", "The resource is locked.", ErrorCategory.Conflict)
        {
            HttpStatusCode = 423
        };

        // Act
        var statusCode = error.HttpStatusCode;

        // Assert
        Assert.Equal(423, statusCode);
    }

    [Fact(DisplayName = "Implicitly converting an ApplicationError to int yields its Code")]
    public void ImplicitConversionToIntYieldsCode()
    {
        // Arrange
        var error = new ApplicationError(9003, "SomeError", "Some Error", "Something went wrong.", ErrorCategory.Internal);

        // Act
        int code = error;

        // Assert
        Assert.Equal(9003, code);
    }
}
