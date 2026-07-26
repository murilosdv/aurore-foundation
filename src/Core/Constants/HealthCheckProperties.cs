namespace Aurore.Foundation.Core.Constants;

/// <summary>
/// Provides well-known constant values used across health check features.
/// </summary>
public static class HealthCheckProperties
{
    /// <summary>
    /// HTTP header names used by health check requests.
    /// </summary>
    public static class Headers
    {
        /// <summary>
        /// The header identifying the client performing the health check request.
        /// </summary>
        public const string Client = "AL-HealthCheck-Client";

        /// <summary>
        /// The header carrying the authorization key for the health check request.
        /// </summary>
        public const string AuthorizationKey = "AL-HealthCheck-Authorization-Key";
    }

    /// <summary>
    /// The standard route paths exposed for health check endpoints.
    /// </summary>
    public static class Endpoints
    {
        /// <summary>
        /// The route path for the liveness probe.
        /// </summary>
        public const string Liveness = "/health/live";

        /// <summary>
        /// The route path for the readiness probe.
        /// </summary>
        public const string Readiness = "/health/ready";

        /// <summary>
        /// The route path for the aggregated status endpoint.
        /// </summary>
        public const string Status = "/health/status";
    }

    /// <summary>
    /// Default configuration values applied to health check infrastructure.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// The default timeout, in seconds, allowed for a health check to complete.
        /// </summary>
        public const int TimeoutInSeconds = 10;

        /// <summary>
        /// The cache key used to store output-cached health check responses.
        /// </summary>
        public const string OutputCacheKey = "health-checks-cache";

        /// <summary>
        /// The rate-limiting policy key applied to health check endpoints.
        /// </summary>
        public const string RateLimitingKey = "health-checks-rate-limiting-checks";
    }

    /// <summary>
    /// Standard tag names used to categorize registered health checks.
    /// </summary>
    public static class Tags
    {
        /// <summary>
        /// Tags a health check as part of the liveness probe.
        /// </summary>
        public const string Liveness = "liveness";

        /// <summary>
        /// Tags a health check as part of the readiness probe.
        /// </summary>
        public const string Readiness = "readiness";

        /// <summary>
        /// Tags a health check as critical to the service's operation.
        /// </summary>
        public const string Critical = "critical";

        /// <summary>
        /// Tags a health check as non-critical to the service's operation.
        /// </summary>
        public const string NonCritical = "non-critical";

        /// <summary>
        /// Tags a health check as verifying a database dependency.
        /// </summary>
        public const string TypeDatabase = "type-database";

        /// <summary>
        /// Tags a health check as verifying an HTTP service dependency.
        /// </summary>
        public const string TypeHttpService = "type-httpservice";
    }
}
