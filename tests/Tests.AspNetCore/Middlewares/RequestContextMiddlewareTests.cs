using System;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Middlewares;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Contexts;
using Aurore.Foundation.TestBed.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.AspNetCore.Middlewares;

public class RequestContextMiddlewareTests
{
    private static DefaultHttpContext CreateContext()
    {
        return HttpContextSetup.Create()
            .WithServices(services => services.AddScoped<RequestContext>())
            .Build();
    }

    [Fact(DisplayName = "InvokeAsync invokes the next delegate and updates the request context")]
    public async Task InvokeAsyncInvokesNextAndUpdatesRequestContext()
    {
        // Arrange
        var context = CreateContext();
        var requestContext = context.RequestServices.GetRequiredService<RequestContext>();
        var nextCallCount = 0;
        var middleware = new RequestContextMiddleware(_ =>
        {
            nextCallCount++;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(1, nextCallCount);
        Assert.NotEmpty(requestContext.CorrelationId);
    }

    [Fact(DisplayName = "A valid correlation id header passes through unchanged")]
    public async Task ValidCorrelationIdHeaderPassesThroughUnchanged()
    {
        // Arrange
        var context = CreateContext();
        var requestContext = context.RequestServices.GetRequiredService<RequestContext>();
        context.Request.Headers[StandardHeaders.CorrelationId] = "abc-123_XYZ.456";
        var middleware = new RequestContextMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal("abc-123_XYZ.456", requestContext.CorrelationId);
    }

    [Fact(DisplayName = "A correlation id header containing an invalid character results in a newly generated id")]
    public async Task InvalidCharacterInHeaderResultsInNewId()
    {
        // Arrange
        var context = CreateContext();
        var requestContext = context.RequestServices.GetRequiredService<RequestContext>();
        context.Request.Headers[StandardHeaders.CorrelationId] = "invalid header@id";
        var middleware = new RequestContextMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.NotEqual("invalid header@id", requestContext.CorrelationId);
        Assert.True(Guid.TryParse(requestContext.CorrelationId, out _));
    }

    [Fact(DisplayName = "A correlation id header longer than 64 characters results in a newly generated id")]
    public async Task HeaderLongerThanMaxLengthResultsInNewId()
    {
        // Arrange
        var context = CreateContext();
        var requestContext = context.RequestServices.GetRequiredService<RequestContext>();
        var tooLong = new string('a', 65);
        context.Request.Headers[StandardHeaders.CorrelationId] = tooLong;
        var middleware = new RequestContextMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.NotEqual(tooLong, requestContext.CorrelationId);
        Assert.True(Guid.TryParse(requestContext.CorrelationId, out _));
    }

    [Fact(DisplayName = "A missing correlation id header results in a newly generated id")]
    public async Task MissingHeaderResultsInNewId()
    {
        // Arrange
        var context = CreateContext();
        var requestContext = context.RequestServices.GetRequiredService<RequestContext>();
        var middleware = new RequestContextMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(Guid.TryParse(requestContext.CorrelationId, out _));
    }

    [Fact(DisplayName = "An idempotency key header populates RequestContext.IdempotencyKey")]
    public async Task IdempotencyKeyHeaderPopulatesRequestContext()
    {
        // Arrange
        var context = CreateContext();
        var requestContext = context.RequestServices.GetRequiredService<RequestContext>();
        context.Request.Headers[StandardHeaders.IdempotencyKey] = "key-123";
        var middleware = new RequestContextMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal("key-123", requestContext.IdempotencyKey);
    }

    [Fact(DisplayName = "A missing idempotency key header leaves RequestContext.IdempotencyKey null")]
    public async Task MissingIdempotencyKeyHeaderLeavesIdempotencyKeyNull()
    {
        // Arrange
        var context = CreateContext();
        var requestContext = context.RequestServices.GetRequiredService<RequestContext>();
        var middleware = new RequestContextMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Null(requestContext.IdempotencyKey);
    }
}
