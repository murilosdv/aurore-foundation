using System.Linq;
using Aurore.Foundation.AspNetCore.MinimalApis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;

namespace Aurore.Foundation.Tests.AspNetCore.MinimalApis;

public class RouteHandlerBuilderExtensionsTests
{
    [Fact(DisplayName = "ProducesProblems declares a problem response for each given status code")]
    public void ProducesProblemsDeclaresEachStatusCode()
    {
        // Arrange
        var app = WebApplication.CreateBuilder().Build();
        app.MapGet("/test", () => "ok").ProducesProblems(400, 404);

        // Act
        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints).Single();
        var statusCodes = endpoint.Metadata
            .OfType<IProducesResponseTypeMetadata>()
            .Select(x => x.StatusCode)
            .ToArray();

        // Assert
        Assert.Contains(400, statusCodes);
        Assert.Contains(404, statusCodes);
    }
}
