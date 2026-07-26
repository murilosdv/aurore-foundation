using System;
using Aurore.Foundation.AspNetCore.Configuration;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tests.AspNetCore.Configuration;

public class CorsConfigurationTests
{
    [Fact(DisplayName = "AddCorsPolicies registers a policy named default as the default CORS policy")]
    public void RegistersDefaultPolicyAsDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        var policies = new[]
        {
            new CorsPolicyOptions { Name = "default", AllowedOrigins = ["https://example.com"] }
        };

        // Act
        services.AddCorsPolicies(policies);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

        // Assert
        var defaultPolicy = options.GetPolicy(options.DefaultPolicyName);
        Assert.NotNull(defaultPolicy);
        Assert.Contains("https://example.com", defaultPolicy.Origins);
    }

    [Fact(DisplayName = "AddCorsPolicies registers a non-default policy under its own name with the configured settings")]
    public void RegistersNonDefaultPolicyWithConfiguredSettings()
    {
        // Arrange
        var services = new ServiceCollection();
        var policies = new[]
        {
            new CorsPolicyOptions
            {
                Name = "partners",
                AllowedHeaders = ["X-Custom-Header"],
                AllowedMethods = ["GET", "POST"],
                AllowedOrigins = ["https://partner.example.com"],
                ExposedHeaders = ["X-Exposed-Header"],
                AllowCredentials = true
            }
        };

        // Act
        services.AddCorsPolicies(policies);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

        // Assert
        var policy = options.GetPolicy("partners");
        Assert.NotNull(policy);
        Assert.Contains("X-Custom-Header", policy.Headers);
        Assert.Contains("GET", policy.Methods);
        Assert.Contains("POST", policy.Methods);
        Assert.Contains("https://partner.example.com", policy.Origins);
        Assert.Contains("X-Exposed-Header", policy.ExposedHeaders);
        Assert.True(policy.SupportsCredentials);
    }

    [Fact(DisplayName = "AddCorsPolicies throws InvalidOperationException when two policies are both named default")]
    public void ThrowsWhenTwoPoliciesAreNamedDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        var policies = new[]
        {
            new CorsPolicyOptions { Name = "default" },
            new CorsPolicyOptions { Name = "default" }
        };

        // Act
        var act = () => services.AddCorsPolicies(policies).BuildServiceProvider().GetRequiredService<IOptions<CorsOptions>>().Value;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}
