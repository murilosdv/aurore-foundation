using System.Linq;
using Aurore.Foundation.Core.Pagination;

namespace Aurore.Foundation.Tests.Core.Pagination;

public class PaginationExtensionsTests
{
    [Fact(DisplayName = "ToOffsetPage returns an empty first page when the source is empty")]
    public void ToOffsetPageReturnsEmptyFirstPageForEmptySource()
    {
        // Arrange
        var source = Enumerable.Empty<int>();

        // Act
        var page = source.ToOffsetPage(x => x.ToString(), currentPage: 3, pageSize: 5);

        // Assert
        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalItems);
        Assert.Equal(1, page.CurrentPage);
    }

    [Fact(DisplayName = "ToOffsetPage transforms every item and counts the full source as TotalItems")]
    public void ToOffsetPageTransformsItemsAndCountsTotal()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var page = source.ToOffsetPage(x => x * 10, currentPage: 2, pageSize: 3);

        // Assert
        Assert.Equal([10, 20, 30], page.Items);
        Assert.Equal(3, page.TotalItems);
        Assert.Equal(2, page.CurrentPage);
    }

    [Fact(DisplayName = "ToCursorPage returns an empty, non-continuable page when the source is empty")]
    public void ToCursorPageReturnsEmptyPageForEmptySource()
    {
        // Arrange
        var source = Enumerable.Empty<int>();

        // Act
        var page = source.ToCursorPage(x => x.ToString(), lastId: "should-be-ignored", hasMore: true, totalItems: 99);

        // Assert
        Assert.Empty(page.Items);
        Assert.Null(page.LastId);
        Assert.False(page.HasMore);
        Assert.Equal(0, page.TotalItems);
    }

    [Fact(DisplayName = "ToCursorPage transforms every item and preserves the given cursor metadata")]
    public void ToCursorPagePreservesCursorMetadata()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var page = source.ToCursorPage(x => x * 10, lastId: "cursor-3", hasMore: true, totalItems: 42);

        // Assert
        Assert.Equal([10, 20, 30], page.Items);
        Assert.Equal("cursor-3", page.LastId);
        Assert.True(page.HasMore);
        Assert.Equal(42, page.TotalItems);
    }
}
