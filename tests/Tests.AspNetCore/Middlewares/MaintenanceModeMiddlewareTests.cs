using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Middlewares;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Http;
using Tests.Shared.Fakes;
using Tests.Shared.Http;

namespace Tests.AspNetCore.Middlewares;

public class MaintenanceModeMiddlewareTests
{
    [Fact(DisplayName = "InvokeAsync invokes next and does not return 503 when maintenance mode is disabled")]
    public async Task DisabledInvokesNext()
    {
        // Arrange
        var context = HttpContextSetup.Create().WithBody(new MemoryStream()).Build();
        var nextCalled = false;
        var middleware = new MaintenanceModeMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, new FakeOptionsMonitor<MaintenanceModeOptions>(new MaintenanceModeOptions { Enabled = false }));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
    }

    [Fact(DisplayName = "InvokeAsync bypasses maintenance mode for requests under /health")]
    public async Task EnabledBypassesHealthPath()
    {
        // Arrange
        var context = HttpContextSetup.Create().WithBody(new MemoryStream()).WithPath("/health/live").Build();
        var nextCalled = false;
        var middleware = new MaintenanceModeMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, new FakeOptionsMonitor<MaintenanceModeOptions>(new MaintenanceModeOptions { Enabled = true }));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
    }

    [Fact(DisplayName = "InvokeAsync short-circuits with a 503 problem response for any other path")]
    public async Task EnabledShortCircuitsOtherPaths()
    {
        // Arrange
        var body = new MemoryStream();
        var context = HttpContextSetup.Create().WithBody(body).WithPath("/api/widgets").Build();
        var nextCalled = false;
        var middleware = new MaintenanceModeMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, new FakeOptionsMonitor<MaintenanceModeOptions>(new MaintenanceModeOptions { Enabled = true, RetryAfterSeconds = 120, Message = "Down for maintenance." }));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
        Assert.Equal("120", context.Response.Headers.RetryAfter);

        body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(body, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("Down for maintenance.", document.RootElement.GetProperty("detail").GetString());
    }

    [Fact(DisplayName = "InvokeAsync uses a default message when none is configured")]
    public async Task EnabledUsesDefaultMessageWhenNoneConfigured()
    {
        // Arrange
        var body = new MemoryStream();
        var context = HttpContextSetup.Create().WithBody(body).WithPath("/api/widgets").Build();
        var middleware = new MaintenanceModeMiddleware(_ => Task.CompletedTask,
            new FakeOptionsMonitor<MaintenanceModeOptions>(new MaintenanceModeOptions { Enabled = true }));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(body, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("The service is temporarily unavailable for maintenance.", document.RootElement.GetProperty("detail").GetString());
    }
}
