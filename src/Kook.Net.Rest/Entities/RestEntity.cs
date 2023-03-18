namespace Kook.Rest;

/// <summary>
///     Represents a generic REST-based entity.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class RestEntity<T> : IEntity<T>
    where T : IEquatable<T>
{
    internal BaseKookClient Kook { get; }

    /// <inheritdoc />
    public T Id { get; }

    internal RestEntity(BaseKookClient kook, T id)
    {
        Kook = kook;
        Id = id;
    }
}
