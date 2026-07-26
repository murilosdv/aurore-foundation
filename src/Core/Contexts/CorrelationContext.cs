namespace Aurore.Foundation.Core.Contexts;

/// <summary>
/// Holds the correlation and distributed-tracing identifiers associated with the current request.
/// </summary>
public sealed class CorrelationContext
{
    /// <summary>
    /// Gets the correlation identifier of the current request.
    /// </summary>
    public string CorrelationId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the distributed trace identifier of the current request.
    /// </summary>
    public string TraceId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the identifier of the current tracing span, or <see langword="null"/> if none is set.
    /// </summary>
    public string? SpanId { get; private set; }

    /// <summary>
    /// Gets the identifier of the first span recorded for this context, preserved across subsequent updates.
    /// </summary>
    public string? OriginalSpanId { get; private set; }

    /// <summary>
    /// Updates the correlation, trace, and span identifiers for the current request.
    /// </summary>
    /// <param name="correlationId">The correlation identifier to set.</param>
    /// <param name="traceId">The distributed trace identifier to set.</param>
    /// <param name="spanId">The span identifier to set. Recorded as <see cref="OriginalSpanId"/> the first time this is called.</param>
    public void Update(string correlationId, string traceId, string? spanId)
    {
        CorrelationId = correlationId;
        TraceId = traceId;
        SpanId = spanId;
        OriginalSpanId ??= spanId;
    }

    /// <summary>
    /// Updates only the current span identifier, leaving <see cref="OriginalSpanId"/> unchanged.
    /// </summary>
    /// <param name="spanId">The new span identifier.</param>
    public void UpdateSpan(string? spanId)
    {
        SpanId = spanId;
    }
}
