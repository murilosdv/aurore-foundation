using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.AspNetCore.Configuration;

/// <summary>
/// Provides extension methods for configuring response compression.
/// </summary>
public static class CompressionConfiguration
{
    /// <summary>
    /// Registers response compression with Gzip and Brotli providers at optimal compression level, enabled over HTTPS,
    /// and extended to cover JSON, Problem+JSON, and icon media types in addition to the default MIME types.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection AddDefaultResponseCompression(this IServiceCollection services)
    {
        return services
            .AddResponseCompression()
            .Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
            .Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
            .Configure<ResponseCompressionOptions>(options =>
            {
                options.EnableForHttps = true;

                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
                    MediaTypeNames.Application.Json,
                    MediaTypeNames.Application.ProblemJson,
                    MediaTypeNames.Image.Icon,
                ]);

                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
            });
    }
}
