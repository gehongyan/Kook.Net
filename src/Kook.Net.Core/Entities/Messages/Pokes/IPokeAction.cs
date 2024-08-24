namespace Kook;

/// <summary>
///     表示一个通用的 POKE 动作。
/// </summary>
public interface IPokeAction
{
    /// <summary>
    ///     获取执行此动作的用户。
    /// </summary>
    IUser Operator { get; }

    /// <summary>
    ///     获取此动作的目标用户。
    /// </summary>
    IReadOnlyCollection<IUser> Targets { get; }

    /// <summary>
    ///     获取此动作关联的 POKE。
    /// </summary>
    IPoke Poke { get; }
}
