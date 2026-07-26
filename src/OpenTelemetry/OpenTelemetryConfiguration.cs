using System;
using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.Core.Options;
using Humanizer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Aurore.Foundation.OpenTelemetry;

/// <summary>
/// Provides an extension method for wiring OpenTelemetry-based logging into an <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class OpenTelemetryConfiguration
{
    /// <summary>
    /// Replaces the host's default logging providers with an OpenTelemetry logging provider, configured with
    /// a resource describing the application and an OTLP exporter pointing at the configured collector.
    /// </summary>
    /// <param name="builder">The host application builder to configure.</param>
    /// <param name="appInfo">Application metadata (component, domain, boundary, version, maintainer) used to build the OpenTelemetry resource.</param>
    /// <param name="settings">The OpenTelemetry logging and OTLP exporter settings.</param>
    /// <param name="environmentName">The name of the deployment environment, recorded as the <c>deployment.environment</c> resource attribute.</param>
    /// <returns>The same <paramref name="builder"/> instance, to allow chaining.</returns>
    public static IHostApplicationBuilder AddOpenTelemetryLogging(
        this IHostApplicationBuilder builder,
        AppInfoOptions appInfo,
        OpenTelemetryOptions settings,
        string environmentName)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        builder.Logging.AddOpenTelemetry(options =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(
                    appInfo.Component.Kebaberize(),
                    $"{appInfo.Domain}.{appInfo.Boundary}".Kebaberize(),
                    appInfo.Version,
                    autoGenerateServiceInstanceId: true,
                    serviceInstanceId: Environment.MachineName)
                .AddAttributes([
                    new(OpenTelemetryProperties.Attributes.Standard.EnvironmentName, environmentName),
                    new(OpenTelemetryProperties.Attributes.Custom.ServiceDomain, appInfo.Domain.Kebaberize()),
                    new(OpenTelemetryProperties.Attributes.Custom.ServiceBoundary, appInfo.Boundary.Kebaberize()),
                    new(OpenTelemetryProperties.Attributes.Custom.ServiceMaintainer, appInfo.Maintainer.Name.Kebaberize()),
                ]);

            options.SetResourceBuilder(resourceBuilder);
            options.IncludeScopes = settings.Logging.IncludeScopes;
            options.ParseStateValues = settings.Logging.ParseStateValues;

            if (settings.Logging.AddConsoleExporter)
                options.AddConsoleExporter();

            options.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(settings.CollectorEndpoint);
                x.Protocol = settings.Protocol.ParseToEnum<OtlpExportProtocol>();
                x.Headers = settings.Headers;
            });
        });

        return builder;
    }
}
