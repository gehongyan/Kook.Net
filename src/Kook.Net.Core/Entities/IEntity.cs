namespace Kook;

/// <summary>
///     Represents a generic entity that has a unique identifier.
/// </summary>
/// <typeparam name="TId"> The type of the unique identifier. </typeparam>
public interface IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     Gets the unique identifier for this object.
    /// </summary>
    TId Id { get; }
}
