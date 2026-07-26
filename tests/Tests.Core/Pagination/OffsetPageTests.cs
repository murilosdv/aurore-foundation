using Aurore.Foundation.Core.Pagination;

namespace Tests.Core.Pagination;

public class OffsetPageTests
{
    [Fact(DisplayName = "TotalPages is the ceiling of TotalItems divided by PageSize")]
    public void TotalPagesRoundsUp()
    {
        // Arrange
        var items = new[] { 1, 2, 3 };

        // Act
        var page = new OffsetPage<int>(items, totalItems: 22, currentPage: 1, pageSize: 5);

        // Assert
        Assert.Equal(5, page.TotalPages);
    }

    [Fact(DisplayName = "TotalPages is zero when there are no items")]
    public void TotalPagesIsZeroWhenEmpty()
    {
        // Arrange
        var items = System.Array.Empty<int>();

        // Act
        var page = new OffsetPage<int>(items, totalItems: 0, currentPage: 1);

        // Assert
        Assert.Equal(0, page.TotalPages);
    }

    [Fact(DisplayName = "PageSize values less than 1 are coerced to 10")]
    public void PageSizeBelowOneIsCoercedToTen()
    {
        // Arrange
        var items = new[] { 1, 2, 3 };

        // Act
        var page = new OffsetPage<int>(items, totalItems: 3, currentPage: 1, pageSize: 0);

        // Assert
        Assert.Equal(10, page.PageSize);
    }
}
