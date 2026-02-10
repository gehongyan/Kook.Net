namespace Kook;

/// <summary>
///     表示一个服务器行为限制条件
/// </summary>
public interface IGuildBehaviorRestrictionCondition
{
    /// <summary>
    ///     获取限制条件类型。
    /// </summary>
    GuildRestrictionConditionType Type { get; }
}