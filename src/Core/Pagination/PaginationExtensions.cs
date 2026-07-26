using System;
using System.Collections.Generic;
using System.Linq;


namespace Aurore.Foundation.Core.Pagination;

/// <summary>
/// Provides extension members for projecting sequences into <see cref="OffsetPage{T}"/> and <see cref="CursorPage{T}"/> instances.
/// </summary>
public static class PaginationExtensions
{
    extension<TSource>(IEnumerable<TSource> collection)
    {
        /// <summary>
        /// Projects the entire source sequence into an <see cref="OffsetPage{T}"/>, transforming each element with <paramref name="transform"/>.
        /// </summary>
        /// <typeparam name="T">The type of item in the resulting page.</typeparam>
        /// <param name="transform">A function that maps each source element to the result type.</param>
        /// <param name="currentPage">The 1-based index of this page.</param>
        /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
        /// <returns>An <see cref="OffsetPage{T}"/> wrapping the transformed items, or an empty page (page 1) if the source is empty.</returns>
        public OffsetPage<T> ToOffsetPage<T>(Func<TSource, T> transform, int currentPage, int pageSize = 10)
        {
            if (collection.Any() is false)
                return new OffsetPage<T>([], 0, 1);

            return new OffsetPage<T>([.. collection.Select(transform)], collection.Count(), currentPage, pageSize);
        }

        /// <summary>
        /// Projects the entire source sequence into a <see cref="CursorPage{T}"/>, transforming each element with <paramref name="transform"/>.
        /// </summary>
        /// <typeparam name="T">The type of item in the resulting page.</typeparam>
        /// <param name="transform">A function that maps each source element to the result type.</param>
        /// <param name="lastId">The cursor identifying the last item in this page, used to request the next page.</param>
        /// <param name="hasMore">Whether more items are available beyond this page.</param>
        /// <param name="totalItems">The total number of items across all pages.</param>
        /// <returns>A <see cref="CursorPage{T}"/> wrapping the transformed items, or an empty page if the source is empty.</returns>
        public CursorPage<T> ToCursorPage<T>(Func<TSource, T> transform, string? lastId, bool hasMore, int totalItems)
        {
            if (collection.Any() is false)
                return new CursorPage<T>([], null, false, 0);

            return new CursorPage<T>(collection.Select(transform), lastId, hasMore, totalItems);
        }
    }
}
