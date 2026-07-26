using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aurore.Foundation.AspNetCore.MinimalApis;

/// <summary>
/// Base class for a minimal API resource group: describes the route prefix, tags, supported versions, and shared
/// configuration applied to all endpoints mapped under it.
/// </summary>
public abstract class ResourceGroup
{
    /// <summary>
    /// Gets the type name used to derive <see cref="Name"/> and <see cref="Tags"/>, defaulting to the class name with a trailing "Resource" suffix removed.
    /// </summary>
    public virtual string TypeName => GetType().Name.Replace("Resource", string.Empty);

    /// <summary>
    /// Gets the route segment this group's endpoints are mapped under, derived from <see cref="TypeName"/> in kebab-case by default.
    /// </summary>
    public virtual string Name => TypeName.Kebaberize();

    /// <summary>
    /// Gets a value indicating whether the group's route is prefixed with <c>api/</c> in addition to the version segment. Defaults to <see langword="false"/>.
    /// </summary>
    public virtual bool PrefixWithApi => false;

    /// <summary>
    /// Gets a value indicating whether endpoints in this group require an authenticated user. Defaults to <see langword="true"/>.
    /// </summary>
    public virtual bool RequireAuthorization => true;

    /// <summary>
    /// Gets the name of the authorization policy to require, or <see langword="null"/> to require just an authenticated user.
    /// </summary>
    public virtual string? AuthorizationPolicyName { get; init; }

    /// <summary>
    /// Gets the OpenAPI tags applied to this group's endpoints, defaulting to <see cref="TypeName"/> in Pascal case.
    /// </summary>
    public virtual string[] Tags => [TypeName.Pascalize()];

    /// <summary>
    /// Gets the API versions this group's endpoints support. Defaults to <c>[1]</c>.
    /// </summary>
    public virtual int[] Versions => [1];

    /// <summary>
    /// Gets the HTTP status codes this group's endpoints declare as possible problem responses. Defaults to 500 Internal Server Error.
    /// </summary>
    public virtual int[] Problems => [StatusCodes.Status500InternalServerError];

    /// <summary>
    /// Configures the mapped route group, e.g. to apply shared filters or metadata. The default implementation does nothing.
    /// </summary>
    /// <param name="builder">The route group builder for this resource group.</param>
    public virtual void Configure(RouteGroupBuilder builder) { }
}
