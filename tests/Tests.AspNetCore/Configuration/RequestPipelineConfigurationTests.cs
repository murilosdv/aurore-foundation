using Aurore.Foundation.AspNetCore.Configuration;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aurore.Foundation.Tests.AspNetCore.Configuration;

public class RequestPipelineConfigurationTests
{
    [Fact(DisplayName = "AddForwardedHeaders adds each configured proxy and network, and enables the standard forwarded header kinds")]
    public void AddForwardedHeadersAddsConfiguredProxiesAndNetworks()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new ForwardedHeadersOptions
        {
            KnownProxies = ["10.0.0.1", "10.0.0.2"],
            KnownNetworks = ["192.168.0.0/24"]
        };

        // Act
        services.AddForwardedHeaders(options);
        var provider = services.BuildServiceProvider();
        var result = provider.GetRequiredService<IOptions<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>>().Value;

        // Assert
        Assert.Equal(
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
            result.ForwardedHeaders);
        Assert.Contains(result.KnownProxies, ip => ip.ToString() == "10.0.0.1");
        Assert.Contains(result.KnownProxies, ip => ip.ToString() == "10.0.0.2");
        Assert.Contains(result.KnownIPNetworks, network => network.ToString() == "192.168.0.0/24");
    }

    [Fact(DisplayName = "AddForwardedHeaders adds no proxies or networks beyond ASP.NET Core's own defaults when none are configured")]
    public void AddForwardedHeadersWithNoProxiesOrNetworksAddsNone()
    {
        // Arrange
        var services = new ServiceCollection();
        var baselineProxyCount = new Microsoft.AspNetCore.Builder.ForwardedHeadersOptions().KnownProxies.Count;
        var baselineNetworkCount = new Microsoft.AspNetCore.Builder.ForwardedHeadersOptions().KnownIPNetworks.Count;
        var options = new ForwardedHeadersOptions();

        // Act
        services.AddForwardedHeaders(options);
        var provider = services.BuildServiceProvider();
        var result = provider.GetRequiredService<IOptions<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>>().Value;

        // Assert
        Assert.Equal(baselineProxyCount, result.KnownProxies.Count);
        Assert.Equal(baselineNetworkCount, result.KnownIPNetworks.Count);
    }
}
