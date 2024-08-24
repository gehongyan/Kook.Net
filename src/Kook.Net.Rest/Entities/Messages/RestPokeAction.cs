using System.Collections.Immutable;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的 POKE 动作。
/// </summary>
public class RestPokeAction : IPokeAction
{
    /// <inheritdoc />
    public IUser Operator { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<IUser> Targets { get; }

    /// <inheritdoc cref="P:Kook.IPokeAction.Poke" />
    public Poke Poke { get; }

    internal RestPokeAction(IUser @operator, IEnumerable<IUser> targets, Poke poke)
    {
        Operator = @operator;
        Targets = targets.ToImmutableArray();
        Poke = poke;
    }

    internal static RestPokeAction Create(BaseKookClient kook, IUser @operator, IEnumerable<IUser> targets, API.Poke poke)
    {
        Poke restPoke = Poke.Create(poke);
        return new RestPokeAction(@operator, targets, restPoke);
    }

    #region IPokeAction

    /// <inheritdoc />
    IPoke IPokeAction.Poke => Poke;

    #endregion
}
