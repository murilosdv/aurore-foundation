using System;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Middlewares;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Contexts;
using Aurore.Foundation.TestBed.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.AspNetCore.Middlewares;

public class CorrelationMiddlewareTests
{
    private static DefaultHttpContext CreateContext()
    {
        return HttpContextSetup.Create()
            .WithServices(services => services.AddScoped<CorrelationContext>())
            .Build();
    }

    [Fact(DisplayName = "InvokeAsync invokes the next delegate and updates the correlation context")]
    public async Task InvokeAsyncInvokesNextAndUpdatesCorrelationContext()
    {
        // Arrange
        var context = CreateContext();
        var correlationContext = context.RequestServices.GetRequiredService<CorrelationContext>();
        var nextCallCount = 0;
        var middleware = new CorrelationMiddleware(_ =>
        {
            nextCallCount++;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(1, nextCallCount);
        Assert.NotEmpty(correlationContext.CorrelationId);
    }

    [Fact(DisplayName = "A valid correlation id header passes through unchanged")]
    public async Task ValidCorrelationIdHeaderPassesThroughUnchanged()
    {
        // Arrange
        var context = CreateContext();
        var correlationContext = context.RequestServices.GetRequiredService<CorrelationContext>();
        context.Request.Headers[StandardHeaders.CorrelationId] = "abc-123_XYZ.456";
        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal("abc-123_XYZ.456", correlationContext.CorrelationId);
    }

    [Fact(DisplayName = "A correlation id header containing an invalid character results in a newly generated id")]
    public async Task InvalidCharacterInHeaderResultsInNewId()
    {
        // Arrange
        var context = CreateContext();
        var correlationContext = context.RequestServices.GetRequiredService<CorrelationContext>();
        context.Request.Headers[StandardHeaders.CorrelationId] = "invalid header@id";
        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.NotEqual("invalid header@id", correlationContext.CorrelationId);
        Assert.True(Guid.TryParse(correlationContext.CorrelationId, out _));
    }

    [Fact(DisplayName = "A correlation id header longer than 64 characters results in a newly generated id")]
    public async Task HeaderLongerThanMaxLengthResultsInNewId()
    {
        // Arrange
        var context = CreateContext();
        var correlationContext = context.RequestServices.GetRequiredService<CorrelationContext>();
        var tooLong = new string('a', 65);
        context.Request.Headers[StandardHeaders.CorrelationId] = tooLong;
        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.NotEqual(tooLong, correlationContext.CorrelationId);
        Assert.True(Guid.TryParse(correlationContext.CorrelationId, out _));
    }

    [Fact(DisplayName = "A missing correlation id header results in a newly generated id")]
    public async Task MissingHeaderResultsInNewId()
    {
        // Arrange
        var context = CreateContext();
        var correlationContext = context.RequestServices.GetRequiredService<CorrelationContext>();
        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(Guid.TryParse(correlationContext.CorrelationId, out _));
    }
}
