namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的具有唯一标识符的实体。
/// </summary>
/// <typeparam name="TId"> 唯一标识符的类型。 </typeparam>
public abstract class RestEntity<TId> : IEntity<TId>
    where TId : IEquatable<TId>
{
    internal BaseKookClient Kook { get; }

    /// <inheritdoc />
    public TId Id { get; }

    internal RestEntity(BaseKookClient kook, TId id)
    {
        Kook = kook;
        Id = id;
    }
}
