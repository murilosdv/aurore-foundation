using System.ComponentModel;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Aurore.Foundation.AspNetCore.MinimalApis;

/// <summary>
/// Base class for a minimal API endpoint belonging to the resource group <typeparamref name="TResourceGroup"/>.
/// Deriving from this generic form lets <see cref="MinimalApiConfiguration.MapResourcesFromAssembly{TAssembly}"/>
/// associate the endpoint with its owning group by reflection.
/// </summary>
/// <typeparam name="TResourceGroup">The resource group this endpoint is mapped under.</typeparam>
public abstract class MinimalEndpoint<TResourceGroup> : MinimalEndpoint
    where TResourceGroup : ResourceGroup;

/// <summary>
/// Base class for a minimal API endpoint: describes how it is mapped, versioned, named, and documented.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class MinimalEndpoint
{
    /// <summary>
    /// Gets the API version this endpoint is mapped to. Defaults to 1.
    /// </summary>
    public virtual int Version { get; } = 1;

    /// <summary>
    /// Gets the endpoint's route name, or <see langword="null"/> to derive one from <see cref="Summary"/>.
    /// </summary>
    public virtual string? Name { get; }

    /// <summary>
    /// Gets the human-readable summary used for API documentation. Defaults to the endpoint's type name, with a
    /// trailing "Endpoint" suffix removed and title-cased.
    /// </summary>
    public virtual string Summary => GetType().Name.Replace("Endpoint", string.Empty).Titleize();

    /// <summary>
    /// Gets the human-readable description used for API documentation, or <see langword="null"/> if none is provided.
    /// </summary>
    public virtual string? Description { get; }

    /// <summary>
    /// Gets the route name used to register and reference this endpoint, derived from <see cref="Name"/> or <see cref="Summary"/> when not explicitly set.
    /// </summary>
    public string RouteName => Name ?? Summary?.Pascalize() ?? "Endpoint";

    /// <summary>
    /// Maps this endpoint's route(s) on the given route builder.
    /// </summary>
    /// <param name="builder">The route builder to map the endpoint on.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> for the mapped route, so further configuration can be chained.</returns>
    public abstract RouteHandlerBuilder Map(IEndpointRouteBuilder builder);
}
