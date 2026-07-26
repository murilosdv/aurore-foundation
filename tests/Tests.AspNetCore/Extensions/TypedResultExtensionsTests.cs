using System.Collections.Generic;
using Aurore.Foundation.AspNetCore.Extensions;
using Aurore.Foundation.AspNetCore.MinimalApis;
using Aurore.Foundation.Core.Errors;
using Aurore.Foundation.Core.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Tests.AspNetCore.Extensions;

public class TypedResultExtensionsTests
{
    private sealed record TestValue(int Id);

    private sealed class TestCreatedEndpoint : MinimalEndpoint
    {
        public override RouteHandlerBuilder Map(IEndpointRouteBuilder builder)
        {
            throw new System.NotSupportedException();
        }
    }

    [Fact(DisplayName = "Created<T> produces a Created result with no location and the given value")]
    public void CreatedProducesResultWithNoLocation()
    {
        // Arrange
        var value = new TestValue(1);

        // Act
        var result = TypedResults.Created(value);

        // Assert
        var created = Assert.IsType<Created<TestValue>>(result);
        Assert.True(string.IsNullOrEmpty(created.Location));
        Assert.Equal(value, created.Value);
    }

    [Fact(DisplayName = "Accepted<T> produces an Accepted result with no location and the given value")]
    public void AcceptedProducesResultWithNoLocation()
    {
        // Arrange
        var value = new TestValue(1);

        // Act
        var result = TypedResults.Accepted(value);

        // Assert
        var accepted = Assert.IsType<Accepted<TestValue>>(result);
        Assert.True(string.IsNullOrEmpty(accepted.Location));
        Assert.Equal(value, accepted.Value);
    }

    [Fact(DisplayName = "Problem(Result) uses the result's Error when set")]
    public void ProblemUsesResultErrorWhenSet()
    {
        // Arrange
        Result result = StandardErrors.NotFound;

        // Act
        var response = TypedResults.Problem(result);

        // Assert
        var problem = Assert.IsAssignableFrom<ProblemHttpResult>(response);
        Assert.Equal(StandardErrors.NotFound.Title, problem.ProblemDetails.Title);
        Assert.Equal(StandardErrors.NotFound.HttpStatusCode, problem.ProblemDetails.Status);
    }

    [Fact(DisplayName = "Problem(Result) falls back to StandardErrors.Validation when only Errors is set")]
    public void ProblemFallsBackToValidationWhenNoErrorSet()
    {
        // Arrange
        var result = Result.Failure(new Dictionary<string, object?> { ["field"] = "is required" });

        // Act
        var response = TypedResults.Problem(result);

        // Assert
        var problem = Assert.IsAssignableFrom<ProblemHttpResult>(response);
        Assert.Equal(StandardErrors.Validation.Title, problem.ProblemDetails.Title);
    }

    [Fact(DisplayName = "ValidationError produces a validation problem carrying the given extensions")]
    public void ValidationErrorProducesValidationProblemWithExtensions()
    {
        // Arrange
        var errors = new Dictionary<string, object?> { ["field"] = "is required" };

        // Act
        var response = TypedResults.ValidationError(errors);

        // Assert
        var problem = Assert.IsAssignableFrom<ProblemHttpResult>(response);
        Assert.Equal(StandardErrors.Validation.Title, problem.ProblemDetails.Title);
        Assert.Equal("is required", problem.ProblemDetails.Extensions["field"]);
    }

    [Fact(DisplayName = "Problem(ApplicationError, extensions) defaults Detail, Type, and Status from the error")]
    public void ProblemWithExtensionsDefaultsFromError()
    {
        // Arrange
        var error = StandardErrors.Conflict;
        var extensions = new Dictionary<string, object?> { ["resourceId"] = "42" };

        // Act
        var problem = TypedResults.Problem(error, extensions);

        // Assert
        Assert.Equal(error.Detail, problem.ProblemDetails.Detail);
        Assert.Equal($"https://aurorelabs.com/docs/errors/{error.Code}", problem.ProblemDetails.Type);
        Assert.Equal(error.HttpStatusCode, problem.ProblemDetails.Status);
        Assert.Equal("42", problem.ProblemDetails.Extensions["resourceId"]);
    }

    [Fact(DisplayName = "Problem(ApplicationError, detail, extensions, instance) uses the error's Detail when none is given")]
    public void ProblemFourArgUsesErrorDetailByDefault()
    {
        // Arrange
        var error = StandardErrors.Locked;

        // Act
        var problem = TypedResults.Problem(error);

        // Assert
        Assert.Equal(error.Detail, problem.ProblemDetails.Detail);
        Assert.Equal($"https://aurorelabs.com/docs/errors/{error.Code}", problem.ProblemDetails.Type);
        Assert.Equal(error.HttpStatusCode, problem.ProblemDetails.Status);
    }

    [Fact(DisplayName = "Problem(ApplicationError, detail, extensions, instance) uses the given detail when overridden")]
    public void ProblemFourArgUsesOverriddenDetail()
    {
        // Arrange
        var error = StandardErrors.Locked;

        // Act
        var problem = TypedResults.Problem(error, "custom detail", null, "instance-1");

        // Assert
        Assert.Equal("custom detail", problem.ProblemDetails.Detail);
        Assert.Equal("instance-1", problem.ProblemDetails.Instance);
    }

    [Fact(DisplayName = "CreatedAt<TEndpoint> points at the endpoint's route name and version")]
    public void CreatedAtUsesEndpointRouteNameAndVersion()
    {
        // Arrange
        var value = new { Id = 1 };

        // Act
        var result = TypedResults.CreatedAt<TestCreatedEndpoint>(value);

        // Assert
        var createdAtRoute = Assert.IsType<CreatedAtRoute<object>>(result);
        var endpoint = new TestCreatedEndpoint();
        Assert.Equal(endpoint.RouteName, createdAtRoute.RouteName);
        Assert.Equal(endpoint.Version.ToString(), createdAtRoute.RouteValues["version"]);
    }
}
