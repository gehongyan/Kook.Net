namespace Kook;

/// <summary>
///     Represents a single poke action.
/// </summary>
public interface IPokeAction
{
    /// <summary>
    ///     Gets the user who performed the action.
    /// </summary>
    IUser Operator { get; }

    /// <summary>
    ///     Gets the users who were poked by the action.
    /// </summary>
    IReadOnlyCollection<IUser> Targets { get; }

    /// <summary>
    ///     Gets the poke this action is associated with.
    /// </summary>
    IPoke Poke { get; }
}
