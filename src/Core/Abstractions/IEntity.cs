namespace Aurore.Foundation.Core.Abstractions;

/// <summary>
/// Represents an entity identified by an <see cref="int"/> key.
/// </summary>
public interface IEntity : IEntity<int>;

/// <summary>
/// Represents an entity identified by a key of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the entity's identifier.</typeparam>
public interface IEntity<T> where T : struct
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    T Id { get; init; }
}
