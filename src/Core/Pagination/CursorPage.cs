using System.Collections.Generic;

namespace Aurore.Foundation.Core.Pagination;

/// <summary>
/// Represents a page of results retrieved using cursor-based (keyset) pagination.
/// </summary>
/// <typeparam name="T">The type of item contained in the page.</typeparam>
public sealed record CursorPage<T>
{
    /// <summary>
    /// Initializes a new <see cref="CursorPage{T}"/>.
    /// </summary>
    /// <param name="items">The items included in this page.</param>
    /// <param name="lastId">The cursor identifying the last item in this page, used to request the next page.</param>
    /// <param name="hasMore">Whether more items are available beyond this page.</param>
    /// <param name="totalItems">The total number of items across all pages.</param>
    public CursorPage(IEnumerable<T> items, string? lastId, bool hasMore, int totalItems)
    {
        Items = [.. items];
        LastId = lastId;
        HasMore = hasMore;
        TotalItems = totalItems;
    }

    /// <summary>
    /// Gets the items included in this page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; init; } = [];

    /// <summary>
    /// Gets the cursor identifying the last item in this page, or <see langword="null"/> if the page is empty.
    /// </summary>
    public string? LastId { get; init; }

    /// <summary>
    /// Gets a value indicating whether more items are available beyond this page.
    /// </summary>
    public bool HasMore { get; init; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalItems { get; init; }
}
