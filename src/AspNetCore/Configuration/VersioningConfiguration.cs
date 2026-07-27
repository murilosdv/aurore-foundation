using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.AspNetCore.Configuration;

/// <summary>
/// Provides extension methods for configuring API versioning.
/// </summary>
[ExcludeFromCodeCoverage]
public static class VersioningConfiguration
{
    /// <summary>
    /// Registers default API versioning: version 1 is assumed when unspecified, versions are reported to callers,
    /// the version is read from the URL segment, and API-explorer group names are formatted as <c>"v{version}"</c>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddDefaultApiVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
