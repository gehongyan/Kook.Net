namespace Kook.Gateway;

/// <summary>
///     Represents a WebSocket-based entity.
/// </summary>
/// <typeparam name="T"> The type of the entity's identifier. </typeparam>
public abstract class SocketEntity<T> : IEntity<T>
    where T : IEquatable<T>
{
    internal KookGatewayClient Kook { get; }

    /// <inheritdoc />
    public T Id { get; }

    internal SocketEntity(KookGatewayClient kook, T id)
    {
        Kook = kook;
        Id = id;
    }
}
