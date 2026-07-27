using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.HealthChecks;
using Aurore.Foundation.Core.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.AspNetCore.HealthChecks;

public class HealthCheckConfigurationTests
{
    private const string ExpectedKey = "expected-key";

    private static (RequestDelegate Endpoint, IServiceProvider Services) MapReadinessEndpoint()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddHealthChecks();
        var app = builder.Build();

        app.MapReadinessEndpoint(HealthCheckProperties.Endpoints.Readiness, ExpectedKey);

        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints).Single();

        return (endpoint.RequestDelegate!, app.Services);
    }

    private static DefaultHttpContext CreateContext(IServiceProvider services)
    {
        return new DefaultHttpContext { RequestServices = services, Response = { Body = new MemoryStream() } };
    }

    [Fact(DisplayName = "MapReadinessEndpoint returns 401 Unauthorized when no authorization key header is present")]
    public async Task ReturnsUnauthorizedWithoutKey()
    {
        // Arrange
        var (endpoint, services) = MapReadinessEndpoint();
        var context = CreateContext(services);

        // Act
        await endpoint(context);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact(DisplayName = "MapReadinessEndpoint returns 401 Unauthorized when the wrong authorization key is presented")]
    public async Task ReturnsUnauthorizedWithWrongKey()
    {
        // Arrange
        var (endpoint, services) = MapReadinessEndpoint();
        var context = CreateContext(services);
        context.Request.Headers[HealthCheckProperties.Headers.AuthorizationKey] = "wrong-key";

        // Act
        await endpoint(context);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact(DisplayName = "MapReadinessEndpoint proceeds to the health check handler and returns 200 when the correct authorization key is presented")]
    public async Task ProceedsWithCorrectKey()
    {
        // Arrange
        var (endpoint, services) = MapReadinessEndpoint();
        var context = CreateContext(services);
        context.Request.Headers[HealthCheckProperties.Headers.AuthorizationKey] = ExpectedKey;

        // Act
        await endpoint(context);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }
}
