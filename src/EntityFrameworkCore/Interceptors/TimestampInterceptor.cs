using System;
using System.Threading;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Aurore.Foundation.EntityFrameworkCore.Interceptors;

/// <summary>
/// A <see cref="SaveChangesInterceptor"/> that stamps <see cref="ITimestampedEntity"/> entries with the current
/// UTC time before changes are saved: <c>CreatedAt</c> is set for added entities, and <c>UpdatedAt</c> is set
/// for both added and modified entities.
/// </summary>
public sealed class TimestampInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Applies creation/update timestamps to tracked <see cref="ITimestampedEntity"/> entries before the
    /// synchronous save operation proceeds.
    /// </summary>
    /// <param name="eventData">Contextual information about the save operation, including the <see cref="DbContext"/> being saved.</param>
    /// <param name="result">The interception result passed in from earlier interceptors.</param>
    /// <returns>The unmodified <paramref name="result"/>, as returned by the base implementation.</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyTimestamps(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Applies creation/update timestamps to tracked <see cref="ITimestampedEntity"/> entries before the
    /// asynchronous save operation proceeds.
    /// </summary>
    /// <param name="eventData">Contextual information about the save operation, including the <see cref="DbContext"/> being saved.</param>
    /// <param name="result">The interception result passed in from earlier interceptors.</param>
    /// <param name="cancellationToken">A token used to observe cancellation requests.</param>
    /// <returns>A task producing the unmodified <paramref name="result"/>, as returned by the base implementation.</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ApplyTimestamps(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplyTimestamps(DbContext? context)
    {
        if (context is null)
            return;

        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<ITimestampedEntity>())
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                if (entry.State is EntityState.Added)
                    entry.Property(x => x.CreatedAt).CurrentValue = now;

                entry.Property(x => x.UpdatedAt).CurrentValue = now;
            }
    }
}
