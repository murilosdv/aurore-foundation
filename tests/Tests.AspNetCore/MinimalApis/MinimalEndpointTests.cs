using Aurore.Foundation.AspNetCore.MinimalApis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Tests.AspNetCore.MinimalApis;

public class MinimalEndpointTests
{
    private sealed class GetWidgetEndpoint : MinimalEndpoint
    {
        public override RouteHandlerBuilder Map(IEndpointRouteBuilder builder)
        {
            throw new System.NotSupportedException();
        }
    }

    private sealed class NamedEndpoint : MinimalEndpoint
    {
        public override string? Name => "CustomRouteName";

        public override RouteHandlerBuilder Map(IEndpointRouteBuilder builder)
        {
            throw new System.NotSupportedException();
        }
    }

    [Fact(DisplayName = "Summary defaults to the type name with the Endpoint suffix removed and title-cased")]
    public void SummaryDefaultsToTitleizedTypeName()
    {
        // Arrange
        var endpoint = new GetWidgetEndpoint();

        // Act
        var summary = endpoint.Summary;

        // Assert
        Assert.Equal("Get Widget", summary);
    }

    [Fact(DisplayName = "RouteName defaults to Summary in Pascal case when Name is not overridden")]
    public void RouteNameDefaultsToPascalizedSummary()
    {
        // Arrange
        var endpoint = new GetWidgetEndpoint();

        // Act
        var routeName = endpoint.RouteName;

        // Assert
        Assert.Equal("GetWidget", routeName);
    }

    [Fact(DisplayName = "RouteName uses Name directly when it is overridden")]
    public void RouteNameUsesOverriddenName()
    {
        // Arrange
        var endpoint = new NamedEndpoint();

        // Act
        var routeName = endpoint.RouteName;

        // Assert
        Assert.Equal("CustomRouteName", routeName);
    }

    [Fact(DisplayName = "Version defaults to 1")]
    public void VersionDefaultsToOne()
    {
        // Arrange
        var endpoint = new GetWidgetEndpoint();

        // Act
        var version = endpoint.Version;

        // Assert
        Assert.Equal(1, version);
    }
}
