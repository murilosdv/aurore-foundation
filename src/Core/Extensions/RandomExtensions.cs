using System;
using System.Collections.Generic;
using System.Linq;

namespace Aurore.Foundation.Core.Extensions;

/// <summary>
/// Provides extension members for <see cref="Random"/>.
/// </summary>
public static class RandomExtensions
{
    extension(Random)
    {
        /// <summary>
        /// Generates a sequence of consecutive integers starting at 1, whose length is a random number of elements
        /// chosen using <see cref="Random.Shared"/> within the given bounds.
        /// </summary>
        /// <param name="min">The inclusive lower bound used to pick the sequence length. Defaults to 1.</param>
        /// <param name="max">The exclusive upper bound used to pick the sequence length. Defaults to 10.</param>
        /// <returns>A sequence of consecutive integers starting at 1, of randomly chosen length.</returns>
        public static IEnumerable<int> Range(int? min = null, int? max = null)
        {
            return Enumerable.Range(1, Random.Shared.Next(min ?? 1, max ?? 10));
        }
    }
}
