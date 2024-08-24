using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的 POKE 动作。
/// </summary>
public class SocketPokeAction : IPokeAction
{
    /// <inheritdoc cref="IPokeAction.Operator" />
    public SocketUser Operator { get; }

    /// <inheritdoc cref="IPokeAction.Targets" />
    public IReadOnlyCollection<SocketUser> Targets { get; }

    /// <inheritdoc cref="IPokeAction.Poke" />
    public Poke Poke { get; private set; }

    internal SocketPokeAction(SocketUser @operator, IEnumerable<SocketUser> targets, Poke poke)
    {
        Operator = @operator;
        Targets = [..targets];
        Poke = poke;
    }

    internal static SocketPokeAction Create(KookSocketClient kook,
        SocketUser @operator, IEnumerable<SocketUser> targets, API.Poke poke)
    {
        Poke restPoke = Poke.Create(poke);
        return new SocketPokeAction(@operator, targets, restPoke);
    }

    #region IPokeAction

    /// <inheritdoc />
    IUser IPokeAction.Operator => Operator;

    /// <inheritdoc />
    IReadOnlyCollection<IUser> IPokeAction.Targets => Targets;

    /// <inheritdoc />
    IPoke IPokeAction.Poke => Poke;

    #endregion
}
