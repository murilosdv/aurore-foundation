using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.AspNetCore.Configuration;

/// <summary>
/// Provides extension methods for configuring JWT bearer authentication and authorization.
/// </summary>
public static class AuthenticationConfiguration
{
    /// <summary>
    /// Registers JWT bearer as the default authentication and challenge scheme, configured from the given identity provider.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="identityProvider">The identity provider settings (authority, audience, HTTPS metadata requirement) used to validate bearer tokens.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddJwtBearerAuthentication(
        this IServiceCollection services,
        IdentityProviderOptions identityProvider)
    {
        return services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = identityProvider.AuthorityUrl;
                options.Audience = identityProvider.Audience;
                options.RequireHttpsMetadata = identityProvider.RequireHttpsMetadata;
            })
            .Services;
    }

    /// <summary>
    /// Registers an authorization policy, named after the JWT bearer scheme, that requires an authenticated user.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddDefaultJwtBearerAuthorization(this IServiceCollection services)
    {
        return services
            .AddAuthorization(options => options
                .AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy => policy
                    .RequireAuthenticatedUser()));
    }
}
