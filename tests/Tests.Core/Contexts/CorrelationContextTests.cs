using Aurore.Foundation.Core.Contexts;

namespace Aurore.Foundation.Tests.Core.Contexts;

public class CorrelationContextTests
{
    [Fact(DisplayName = "Update sets the correlation, trace and span identifiers")]
    public void UpdateSetsAllIdentifiers()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act
        context.Update("correlation-1", "trace-1", "span-1");

        // Assert
        Assert.Equal("correlation-1", context.CorrelationId);
        Assert.Equal("trace-1", context.TraceId);
        Assert.Equal("span-1", context.SpanId);
    }

    [Fact(DisplayName = "OriginalSpanId is recorded on the first Update and preserved across later ones")]
    public void OriginalSpanIdIsStickyAcrossUpdates()
    {
        // Arrange
        var context = new CorrelationContext();

        // Act
        context.Update("correlation-1", "trace-1", "span-1");
        context.Update("correlation-1", "trace-1", "span-2");

        // Assert
        Assert.Equal("span-1", context.OriginalSpanId);
        Assert.Equal("span-2", context.SpanId);
    }

    [Fact(DisplayName = "UpdateSpan changes only the current span, leaving OriginalSpanId untouched")]
    public void UpdateSpanLeavesOriginalSpanIdUnchanged()
    {
        // Arrange
        var context = new CorrelationContext();
        context.Update("correlation-1", "trace-1", "span-1");

        // Act
        context.UpdateSpan("span-2");

        // Assert
        Assert.Equal("span-2", context.SpanId);
        Assert.Equal("span-1", context.OriginalSpanId);
    }
}
