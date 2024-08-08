namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的具有唯一标识符的实体。
/// </summary>
/// <typeparam name="TId"> 唯一标识符的类型。 </typeparam>
public abstract class SocketEntity<TId> : IEntity<TId>
    where TId : IEquatable<TId>
{
    internal KookSocketClient Kook { get; }

    /// <inheritdoc />
    public TId Id { get; }

    internal SocketEntity(KookSocketClient kook, TId id)
    {
        Kook = kook;
        Id = id;
    }
}
