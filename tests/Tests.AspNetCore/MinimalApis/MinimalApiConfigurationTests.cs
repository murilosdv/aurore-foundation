using System;
using System.Linq;
using Aurore.Foundation.AspNetCore.MinimalApis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.AspNetCore.MinimalApis;

public class MinimalApiConfigurationTests
{
    private sealed class NoParameterlessConstructor
    {
        public NoParameterlessConstructor(int value)
        {
            _ = value;
        }
    }

    private sealed class ConfiguredResource : ResourceGroup
    {
        public override string[] Tags => ["CustomTag"];
        public override int[] Problems => [StatusCodes.Status404NotFound];
        public override int[] Versions => [1, 2];
    }

    private sealed class PolicyResource : ResourceGroup
    {
        public override string? AuthorizationPolicyName => "MyPolicy";
    }

    private sealed class NoAuthResource : ResourceGroup
    {
        public override bool RequireAuthorization => false;
    }

    private static WebApplication CreateAppWithVersioning()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddApiVersioning();
        return builder.Build();
    }

    [Fact(DisplayName = "CreateInstance throws InvalidOperationException wrapping MissingMethodException for a type with no public parameterless constructor")]
    public void CreateInstanceThrowsForTypeWithoutParameterlessConstructor()
    {
        // Arrange & Act
        var act = () => MinimalApiConfiguration.CreateInstance<NoParameterlessConstructor>(typeof(NoParameterlessConstructor));

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(act);
        Assert.IsType<MissingMethodException>(exception.InnerException);
    }

    [Fact(DisplayName = "ConfigureTags applies the resource group's tags to the route group")]
    public void ConfigureTagsAppliesGroupTags()
    {
        // Arrange
        var app = CreateAppWithVersioning();
        var group = app.MapGroup("test").ConfigureTags(new ConfiguredResource());
        group.MapGet("/x", () => "ok");

        // Act
        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints).Single();
        var tags = endpoint.Metadata.GetMetadata<ITagsMetadata>();

        // Assert
        Assert.NotNull(tags);
        Assert.Contains("CustomTag", tags.Tags);
    }

    [Fact(DisplayName = "ConfigureProblems declares a problem response for each configured status code plus the 500 default")]
    public void ConfigureProblemsDeclaresConfiguredAndDefaultStatusCodes()
    {
        // Arrange
        var app = CreateAppWithVersioning();
        var group = app.MapGroup("test").ConfigureProblems(new ConfiguredResource());
        group.MapGet("/x", () => "ok");

        // Act
        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints).Single();
        var statusCodes = endpoint.Metadata.OfType<IProducesResponseTypeMetadata>().Select(x => x.StatusCode).ToArray();

        // Assert
        Assert.Contains(StatusCodes.Status404NotFound, statusCodes);
        Assert.Contains(StatusCodes.Status500InternalServerError, statusCodes);
    }

    [Fact(DisplayName = "ConfigureVersionSet applies an API version set to the route group without throwing")]
    public void ConfigureVersionSetAppliesVersionSet()
    {
        // Arrange
        var app = CreateAppWithVersioning();
        var group = app.MapGroup("test").ConfigureVersionSet(new ConfiguredResource());
        group.MapGet("/x", () => "ok");

        // Act
        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints).Single();
        var versionMetadata = endpoint.Metadata.OfType<Asp.Versioning.ApiVersionMetadata>().SingleOrDefault();

        // Assert
        Assert.NotNull(versionMetadata);
        Assert.False(versionMetadata.IsApiVersionNeutral);
    }

    [Fact(DisplayName = "ConfigureAuthorization requires an authenticated user with no policy by default")]
    public void ConfigureAuthorizationRequiresAuthenticatedUserByDefault()
    {
        // Arrange
        var app = CreateAppWithVersioning();
        var group = app.MapGroup("test").ConfigureAuthorization(new ConfiguredResource());
        group.MapGet("/x", () => "ok");

        // Act
        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints).Single();
        var authorizeData = endpoint.Metadata.OfType<AuthorizeAttribute>().SingleOrDefault();

        // Assert
        Assert.NotNull(authorizeData);
        Assert.Null(authorizeData.Policy);
    }

    [Fact(DisplayName = "ConfigureAuthorization requires the configured policy name when one is set")]
    public void ConfigureAuthorizationUsesConfiguredPolicyName()
    {
        // Arrange
        var app = CreateAppWithVersioning();
        var group = app.MapGroup("test").ConfigureAuthorization(new PolicyResource());
        group.MapGet("/x", () => "ok");

        // Act
        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints).Single();
        var authorizeData = endpoint.Metadata.OfType<AuthorizeAttribute>().SingleOrDefault();

        // Assert
        Assert.NotNull(authorizeData);
        Assert.Equal("MyPolicy", authorizeData.Policy);
    }

    [Fact(DisplayName = "ConfigureAuthorization does not require authorization when the group opts out")]
    public void ConfigureAuthorizationSkipsWhenNotRequired()
    {
        // Arrange
        var app = CreateAppWithVersioning();
        var group = app.MapGroup("test").ConfigureAuthorization(new NoAuthResource());
        group.MapGet("/x", () => "ok");

        // Act
        var endpoint = ((IEndpointRouteBuilder)app).DataSources.SelectMany(x => x.Endpoints).Single();
        var authorizeData = endpoint.Metadata.OfType<AuthorizeAttribute>().SingleOrDefault();

        // Assert
        Assert.Null(authorizeData);
    }

    [Fact(DisplayName = "MapResourcesFromAssembly maps a resource group's endpoints under its kebab-case route with the group's tags and the endpoint's route name")]
    public void MapResourcesFromAssemblyMapsGroupAndEndpoint()
    {
        // Arrange
        var app = CreateAppWithVersioning();

        // Act
        ((IEndpointRouteBuilder)app).MapResourcesFromAssembly<MinimalApiConfigurationTests>();

        // Assert
        var endpoint = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(x => x.Endpoints)
            .OfType<RouteEndpoint>()
            .Single(x => x.RoutePattern.RawText!.Contains("widgets"));

        Assert.Contains("v{version:apiVersion}/widgets", endpoint.RoutePattern.RawText);

        var tags = endpoint.Metadata.GetMetadata<ITagsMetadata>();
        Assert.NotNull(tags);
        Assert.Contains("Widgets", tags.Tags);

        var name = endpoint.Metadata.GetMetadata<IEndpointNameMetadata>();
        Assert.Equal("GetWidget", name?.EndpointName);
    }
}
