using System;
using System.Collections.Generic;

namespace Aurore.Foundation.Core.Pagination;

/// <summary>
/// Represents a page of results retrieved using offset-based pagination.
/// </summary>
/// <typeparam name="T">The type of item contained in the page.</typeparam>
public sealed record OffsetPage<T>
{
    /// <summary>
    /// Initializes a new <see cref="OffsetPage{T}"/>, computing <see cref="TotalPages"/> from <paramref name="totalItems"/> and <paramref name="pageSize"/>.
    /// </summary>
    /// <param name="items">The items included in this page.</param>
    /// <param name="totalItems">The total number of items across all pages.</param>
    /// <param name="currentPage">The 1-based index of this page.</param>
    /// <param name="pageSize">The number of items per page. Values less than 1 are coerced to 10.</param>
    public OffsetPage(IEnumerable<T> items, int totalItems, int currentPage, int pageSize = 10)
    {
        if (pageSize < 1)
            pageSize = 10;

        Items = [.. items];
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize);
        TotalItems = totalItems;
    }

    /// <summary>
    /// Gets the 1-based index of this page.
    /// </summary>
    public int CurrentPage { get; init; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the total number of pages, computed from <see cref="TotalItems"/> and <see cref="PageSize"/>.
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalItems { get; init; }

    /// <summary>
    /// Gets the items included in this page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; init; } = [];
}
