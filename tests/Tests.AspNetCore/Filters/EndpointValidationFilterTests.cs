using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Filters;
using Aurore.Foundation.TestBed.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.AspNetCore.Filters;

public class EndpointValidationFilterTests
{
    private sealed record Widget(string Name);

    private sealed class WidgetValidator : AbstractValidator<Widget>
    {
        public WidgetValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private static EndpointFilterDelegate Next(object? result)
    {
        return _ => ValueTask.FromResult(result);
    }

    [Fact(DisplayName = "InvokeAsync calls the next delegate when all validated arguments are valid")]
    public async Task CallsNextWhenArgumentsAreValid()
    {
        // Arrange
        var context = HttpContextSetup.Create()
            .WithServices(services => services.AddSingleton<IValidator<Widget>>(new WidgetValidator()))
            .Build();

        var invocationContext = new DefaultEndpointFilterInvocationContext(context, new Widget("gadget"));
        var filter = new EndpointValidationFilter();

        // Act
        var result = await filter.InvokeAsync(invocationContext, Next("ok"));

        // Assert
        Assert.Equal("ok", result);
    }

    [Fact(DisplayName = "InvokeAsync skips arguments with no registered validator and calls the next delegate")]
    public async Task SkipsArgumentsWithoutRegisteredValidator()
    {
        // Arrange
        var context = HttpContextSetup.Create().Build();
        var invocationContext = new DefaultEndpointFilterInvocationContext(context, new Widget(""));
        var filter = new EndpointValidationFilter();

        // Act
        var result = await filter.InvokeAsync(invocationContext, Next("ok"));

        // Assert
        Assert.Equal("ok", result);
    }

    [Fact(DisplayName = "InvokeAsync skips null arguments and calls the next delegate")]
    public async Task SkipsNullArguments()
    {
        // Arrange
        var context = HttpContextSetup.Create()
            .WithServices(services => services.AddSingleton<IValidator<Widget>>(new WidgetValidator()))
            .Build();

        var invocationContext = new DefaultEndpointFilterInvocationContext(context, [null]);
        var filter = new EndpointValidationFilter();

        // Act
        var result = await filter.InvokeAsync(invocationContext, Next("ok"));

        // Assert
        Assert.Equal("ok", result);
    }

    [Fact(DisplayName = "InvokeAsync returns a validation-error problem result when an argument fails validation")]
    public async Task ReturnsValidationErrorWhenArgumentIsInvalid()
    {
        // Arrange
        var context = HttpContextSetup.Create()
            .WithServices(services => services.AddSingleton<IValidator<Widget>>(new WidgetValidator()))
            .Build();

        var invocationContext = new DefaultEndpointFilterInvocationContext(context, new Widget(""));
        var filter = new EndpointValidationFilter();

        // Act
        var result = await filter.InvokeAsync(invocationContext, Next("ok"));

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(400, problem.StatusCode);
    }
}
