namespace Aurore.Foundation.OpenTelemetry;

/// <summary>
/// Well-known names used throughout the Aurore OpenTelemetry integration: <see cref="System.Diagnostics.ActivitySource"/>
/// names, HTTP endpoint paths, and resource/tag attribute keys (both OpenTelemetry semantic-convention
/// and Aurore-specific custom attributes).
/// </summary>
public static class OpenTelemetryProperties
{
    /// <summary>
    /// Names of the shared <see cref="System.Diagnostics.ActivitySource"/>s used to emit traces for
    /// cross-cutting platform concerns that are not tied to a single application's own activity source.
    /// </summary>
    public static class Sources
    {
        /// <summary>
        /// The activity source name used for traces emitted by the messaging infrastructure (e.g. message bus
        /// handlers and publishers).
        /// </summary>
        public const string Messaging = "Aurore.Platform.Messaging";

        /// <summary>
        /// The activity source name used for traces emitted by general platform infrastructure code.
        /// </summary>
        public const string Infrastructure = "Aurore.Platform.Infrastructure";
    }

    /// <summary>
    /// HTTP route paths exposed by the OpenTelemetry integration for external tooling to consume.
    /// </summary>
    public static class Endpoints
    {
        /// <summary>
        /// The route path at which the Prometheus scraping endpoint exposes collected metrics.
        /// </summary>
        public const string Metrics = "/diagnostics/metrics";
    }

    /// <summary>
    /// Attribute (tag/resource) key names used when annotating OpenTelemetry logs, metrics and traces.
    /// </summary>
    public static class Attributes
    {
        /// <summary>
        /// Attribute keys defined by the OpenTelemetry semantic conventions, used as-is so that data
        /// remains compatible with standard OpenTelemetry tooling and backends.
        /// </summary>
        public static class Standard
        {
            /// <summary>
            /// The deployment environment the telemetry was produced in (e.g. <c>production</c>, <c>staging</c>).
            /// </summary>
            public const string EnvironmentName = "deployment.environment";

            /// <summary>
            /// The unique identifier of the specific service instance that produced the telemetry.
            /// </summary>
            public const string ServiceInstanceId = "service.instance.id";

            /// <summary>
            /// The logical name of the service that produced the telemetry.
            /// </summary>
            public const string ServiceName = "service.name";

            /// <summary>
            /// The version of the service that produced the telemetry.
            /// </summary>
            public const string ServiceVersion = "service.version";

            /// <summary>
            /// The HTTP request method (e.g. <c>GET</c>, <c>POST</c>) associated with a span.
            /// </summary>
            public const string HttpMethod = "http.method";

            /// <summary>
            /// The value of the HTTP <c>Host</c> header associated with a span.
            /// </summary>
            public const string HttpHost = "http.host";

            /// <summary>
            /// The full HTTP request target (path and query string) associated with a span.
            /// </summary>
            public const string HttpTarget = "http.target";

            /// <summary>
            /// The path component of the request URL associated with a span.
            /// </summary>
            public const string UrlPath = "url.path";

            /// <summary>
            /// The query-string component of the request URL associated with a span.
            /// </summary>
            public const string UrlQuery = "url.query";

            /// <summary>
            /// The IP address of the remote peer for a network operation.
            /// </summary>
            public const string NetPeerIp = "net.peer.ip";

            /// <summary>
            /// The raw, unparsed <c>User-Agent</c> header value associated with a request.
            /// </summary>
            public const string UserAgentOriginal = "user_agent.original";
        }

        /// <summary>
        /// Aurore-specific attribute keys (prefixed with <c>fl.</c>) that are not part of the OpenTelemetry
        /// semantic conventions, used to enrich telemetry with platform-specific context.
        /// </summary>
        public static class Custom
        {
            /// <summary>
            /// The business domain the emitting application belongs to (see
            /// <see cref="Aurore.Foundation.Core.Options.AppInfoOptions.Domain"/>).
            /// </summary>
            public const string ServiceDomain = "fl.service.domain";

            /// <summary>
            /// The bounded context (boundary) the emitting application belongs to within its domain (see
            /// <see cref="Aurore.Foundation.Core.Options.AppInfoOptions.Boundary"/>).
            /// </summary>
            public const string ServiceBoundary = "fl.service.boundary";

            /// <summary>
            /// The name of the emitting application component (see
            /// <see cref="Aurore.Foundation.Core.Options.AppInfoOptions.Component"/>).
            /// </summary>
            public const string ServiceComponent = "fl.service.component";

            /// <summary>
            /// The name of the team or individual responsible for maintaining the emitting application.
            /// </summary>
            public const string ServiceMaintainer = "fl.service.maintainer";

            /// <summary>
            /// A correlation identifier used to tie together telemetry emitted across multiple services
            /// for a single logical operation.
            /// </summary>
            public const string CorrelationId = "fl.correlation.id";

            /// <summary>
            /// The name of the message handler that processed a given message, used to enrich messaging telemetry.
            /// </summary>
            public const string MessageHandler = "fl.messaging.handler";

            /// <summary>
            /// The identifier of the distributed trace a log record or event belongs to.
            /// </summary>
            public const string TraceId = "trace_id";

            /// <summary>
            /// The identifier of the span within a trace that a log record or event belongs to.
            /// </summary>
            public const string SpanId = "span_id";
        }
    }
}
