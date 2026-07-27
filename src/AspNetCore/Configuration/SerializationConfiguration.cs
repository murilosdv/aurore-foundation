using System.Diagnostics.CodeAnalysis;
using Aurore.Foundation.Core.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aurore.Foundation.AspNetCore.Configuration;

/// <summary>
/// Provides extension methods for configuring JSON serialization defaults.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SerializationConfiguration
{
    /// <summary>
    /// Applies Aurore's default JSON serializer settings to the host's minimal API JSON options.
    /// </summary>
    /// <param name="builder">The host application builder to configure.</param>
    /// <returns>The <see cref="IHostApplicationBuilder"/> so calls can be chained.</returns>
    public static IHostApplicationBuilder ConfigureSerializers(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(o =>
            JsonSerializerConfiguration.Configure(o.SerializerOptions));

        return builder;
    }
}
