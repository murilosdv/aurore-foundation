using Aurore.Foundation.AspNetCore.MinimalApis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Tests.AspNetCore.MinimalApis;

/// <summary>
/// A minimal <see cref="ResourceGroup"/> with no overrides, shared by <see cref="ResourceGroupTests"/> (to exercise
/// default name/tag/route derivation) and <see cref="MinimalApiConfigurationTests"/> (as the group for
/// <see cref="GetWidgetEndpoint"/> in the <c>MapResourcesFromAssembly</c> end-to-end test).
/// </summary>
internal sealed class WidgetsResource : ResourceGroup;

/// <summary>
/// A minimal <see cref="MinimalEndpoint{TResourceGroup}"/> mapped under <see cref="WidgetsResource"/>, used by
/// the <c>MapResourcesFromAssembly</c> end-to-end test in <see cref="MinimalApiConfigurationTests"/>.
/// </summary>
internal sealed class GetWidgetEndpoint : MinimalEndpoint<WidgetsResource>
{
    /// <inheritdoc/>
    public override RouteHandlerBuilder Map(IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/", () => "ok");
    }
}
