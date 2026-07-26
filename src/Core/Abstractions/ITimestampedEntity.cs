using System;

namespace Aurore.Foundation.Core.Abstractions;

/// <summary>
/// Represents an entity that tracks creation and last-update timestamps.
/// </summary>
public interface ITimestampedEntity
{
    /// <summary>
    /// Gets or sets the date and time at which the entity was created.
    /// </summary>
    DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets the date and time at which the entity was last updated.
    /// </summary>
    DateTime UpdatedAt { get; set; }
}
