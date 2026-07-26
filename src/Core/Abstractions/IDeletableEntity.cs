using System;

namespace Aurore.Foundation.Core.Abstractions;

/// <summary>
/// Represents an entity that supports soft deletion.
/// </summary>
public interface IDeletableEntity
{
    /// <summary>
    /// Gets or sets the date and time at which the entity was soft-deleted, or <see langword="null"/> if it has not been deleted.
    /// </summary>
    DateTime? DeletedAt { get; init; }
}
