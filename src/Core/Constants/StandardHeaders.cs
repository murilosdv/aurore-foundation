namespace Aurore.Foundation.Core.Constants;

/// <summary>
/// Provides standard HTTP header names used across Aurore applications.
/// </summary>
public static class StandardHeaders
{
    /// <summary>
    /// The header carrying a request's correlation identifier.
    /// </summary>
    public const string CorrelationId = "Correlation-Id";

    /// <summary>
    /// The header carrying the support key used for elevated diagnostics or support access.
    /// </summary>
    public const string SupportKey = "AL-Support-Key";

    /// <summary>
    /// The standard User-Agent header.
    /// </summary>
    public const string UserAgent = "User-Agent";

    /// <summary>
    /// The header carrying a client-supplied idempotency key.
    /// </summary>
    public const string IdempotencyKey = "Idempotency-Key";

    /// <summary>
    /// The W3C Trace Context header carrying distributed tracing information.
    /// </summary>
    public const string TraceParent = "traceparent";
}
