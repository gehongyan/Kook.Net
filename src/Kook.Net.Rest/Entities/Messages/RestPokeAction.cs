using System.Collections.Immutable;

namespace Kook.Rest;

public class RestPokeAction : IPokeAction
{
    /// <inheritdoc cref="IPokeAction.Operator" />
    public IUser Operator { get; }

    /// <inheritdoc cref="IPokeAction.Targets" />
    public IReadOnlyCollection<IUser> Targets { get; }

    /// <inheritdoc cref="IPokeAction.Poke" />
    public Poke Poke { get; set; }

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
    IUser IPokeAction.Operator => Operator;
    /// <inheritdoc />
    IReadOnlyCollection<IUser> IPokeAction.Targets => Targets;
    /// <inheritdoc />
    IPoke IPokeAction.Poke => Poke;
    #endregion
}
