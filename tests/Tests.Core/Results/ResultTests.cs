using System.Collections.Generic;
using Aurore.Foundation.Core.Errors;
using Aurore.Foundation.Core.Results;

namespace Aurore.Foundation.Tests.Core.Results;

public class ResultTests
{
    [Fact(DisplayName = "Failure produces a failed result carrying the given error details")]
    public void FailureProducesFailedResult()
    {
        // Arrange
        var errors = new Dictionary<string, object?> { ["field"] = "is required" };

        // Act
        var result = Result.Failure(errors);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Same(errors, result.Errors);
    }

    [Fact(DisplayName = "Converting an ApplicationError to a Result carries the error and its extensions")]
    public void ImplicitConversionFromApplicationErrorCarriesErrorAndExtensions()
    {
        // Arrange
        var extensions = new Dictionary<string, object?> { ["traceId"] = "abc-123" };
        var error = StandardErrors.NotFound with { Extensions = extensions };

        // Act
        Result result = error;

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Same(extensions, result.Errors);
    }

    [Fact(DisplayName = "A default Result is successful")]
    public void DefaultResultIsSuccessful()
    {
        // Arrange
        var result = new Result();

        // Act
        var isSuccess = result.IsSuccess;

        // Assert
        Assert.True(isSuccess);
    }

    [Fact(DisplayName = "Converting a value to Result<T> produces a successful result carrying that value")]
    public void ImplicitConversionFromValueProducesSuccessfulResult()
    {
        // Arrange & Act
        Result<string> result = "hello";

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("hello", result.Value);
    }

    [Fact(DisplayName = "Converting an ApplicationError to Result<T> produces a failed result with no value")]
    public void ImplicitConversionFromApplicationErrorProducesFailedResult()
    {
        // Arrange
        var error = StandardErrors.Validation;

        // Act
        Result<string> result = error;

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.Equal(error, result.Error);
    }

    [Fact(DisplayName = "Converting an error dictionary to Result<T> produces a failed result")]
    public void ImplicitConversionFromDictionaryProducesFailedResult()
    {
        // Arrange
        var errors = new Dictionary<string, object?> { ["field"] = "is invalid" };

        // Act
        Result<string> result = errors;

        // Assert
        Assert.True(result.IsFailure);
        Assert.Same(errors, result.Errors);
    }
}
