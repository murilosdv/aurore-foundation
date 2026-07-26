using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.Core.Options;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Aurore.Foundation.OpenTelemetry;

/// <summary>
/// Provides extension methods for wiring the default Aurore OpenTelemetry metrics and tracing pipeline
/// into an ASP.NET Core application.
/// </summary>
public static class OpenTelemetryExtensions
{
    private const string PostgresNamespace = "Npgsql";

    /// <summary>
    /// Registers OpenTelemetry with the dependency injection container, configuring a resource describing the
    /// application, ASP.NET Core/HTTP client/runtime/process/Postgres metrics with Prometheus and OTLP exporters,
    /// and request tracing (with health-check paths excluded) sampled via a parent-based trace-ID-ratio sampler
    /// and exported via OTLP.
    /// </summary>
    /// <param name="services">The service collection to register OpenTelemetry with.</param>
    /// <param name="appInfo">Application metadata (component, domain, boundary, version, maintainer) used to build the OpenTelemetry resource.</param>
    /// <param name="settings">The OpenTelemetry metrics, tracing and OTLP exporter settings.</param>
    /// <param name="activitySourceName">The name of the application's own <see cref="System.Diagnostics.ActivitySource"/> to subscribe to for tracing.</param>
    /// <param name="environmentName">The name of the deployment environment, recorded as the <c>deployment.environment</c> resource attribute.</param>
    /// <returns>The same <paramref name="services"/> instance, to allow chaining.</returns>
    public static IServiceCollection AddDefaultOpenTelemetry(
       this IServiceCollection services,
       AppInfoOptions appInfo,
       OpenTelemetryOptions settings,
       string activitySourceName,
       string environmentName)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
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
                ]))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddMeter(PostgresNamespace)
                    .AddPrometheusExporter()
                    .AddOtlpExporter(x => ConfigureOtlpExporter(x, settings));

                if (settings.Metrics.ExportToConsole)
                    metrics.AddConsoleExporter();
            })
            .WithTracing(tracing =>
            {
                var sampler = new ParentBasedSampler(new TraceIdRatioBasedSampler(settings.Tracing.TraceIdRatioBasedSampler));

                tracing
                    .AddSource(activitySourceName)
                    .AddSource(OpenTelemetryProperties.Sources.Messaging)
                    .AddSource(OpenTelemetryProperties.Sources.Infrastructure)
                    .AddSource(PostgresNamespace)
                    .SetSampler(sampler)
                    .AddIncomingTrafficInstrumentation(settings.Tracing.IncomingTrafficFilters)
                    .AddOutgoingTrafficInstrumentation(settings.Tracing.OutgoingTrafficFilters)
                    .SetErrorStatusOnException()
                    .AddOtlpExporter(x => ConfigureOtlpExporter(x, settings));

                if (settings.Tracing.ExportToConsole)
                    tracing.AddConsoleExporter();
            });

        return services;
    }

    /// <summary>
    /// Maps the default Prometheus scraping endpoint (<see cref="OpenTelemetryProperties.Endpoints.Metrics"/>) so
    /// that collected metrics can be scraped by a Prometheus server.
    /// </summary>
    /// <param name="builder">The application builder to map the endpoint on.</param>
    /// <returns>The same <paramref name="builder"/> instance, to allow chaining.</returns>
    public static IApplicationBuilder UseDefaultPrometheusEndpoint(this IApplicationBuilder builder)
    {
        return builder.UseOpenTelemetryPrometheusScrapingEndpoint(OpenTelemetryProperties.Endpoints.Metrics);
    }

    private static void ConfigureOtlpExporter(OtlpExporterOptions options, OpenTelemetryOptions settings)
    {
        options.Endpoint = new Uri(settings.CollectorEndpoint);
        options.Protocol = settings.Protocol.ParseToEnum<OtlpExportProtocol>();
        options.Headers = settings.Headers;
    }

    private static TracerProviderBuilder AddOutgoingTrafficInstrumentation(
        this TracerProviderBuilder builder,
        IEnumerable<string> pathsFilter,
        Func<HttpRequestMessage, bool>? requestFilter = null)
    {
        var excludedPaths = CreatePathsFilter(pathsFilter);

        return builder
            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException = true;
                options.FilterHttpRequestMessage = request =>
                {
                    if (request.RequestUri is null)
                        return false;

                    var path = request.RequestUri.PathAndQuery.ToLowerInvariant();

                    if (excludedPaths.Any(path.StartsWith))
                        return false;

                    return requestFilter is null || requestFilter(request);
                };
            });
    }

    private static TracerProviderBuilder AddIncomingTrafficInstrumentation(
        this TracerProviderBuilder builder,
        IEnumerable<string> pathsFilter)
    {
        var excludedPaths = CreatePathsFilter(pathsFilter);

        return builder
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.Filter = context =>
                {
                    if (context.Request.Path.Value is null)
                        return false;

                    var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

                    return !excludedPaths.Any(path.StartsWith);
                };
            });
    }

    private static string[] CreatePathsFilter(IEnumerable<string> paths)
    {
        return [.. paths.Select(x => x.ToLowerInvariant()).Concat(["/health/"])];
    }
}
