using System;
using System.Linq;
using Aurore.Foundation.Core.Extensions;

namespace Aurore.Foundation.Tests.Core.Extensions;

public class RandomExtensionsTests
{
    [Fact(DisplayName = "Range with explicit bounds that leave only one possible length produces a deterministic sequence")]
    public void RangeWithExplicitBoundsProducesDeterministicLength()
    {
        // Arrange & Act
        var result = Random.Range(min: 3, max: 4).ToList();

        // Assert
        Assert.Equal([1, 2, 3], result);
    }

    [Fact(DisplayName = "Range with no arguments produces a sequence of consecutive integers starting at 1, between 1 and 9 elements long")]
    public void RangeWithDefaultsProducesSequenceWithinDefaultBounds()
    {
        // Arrange & Act
        var result = Random.Range().ToList();

        // Assert
        Assert.InRange(result.Count, 1, 9);
        Assert.Equal(Enumerable.Range(1, result.Count), result);
    }
}
