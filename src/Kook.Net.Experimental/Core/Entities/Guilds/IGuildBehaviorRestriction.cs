namespace Kook;

/// <summary>
///     表示一个通用的服务器行为限制。
/// </summary>
public interface IGuildBehaviorRestriction : IEntity<string>
{
    /// <summary>
    ///     获取此行为限制所属的服务器的 ID。
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    ///     获取此限制的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     获取限制条件类型。
    /// </summary>
    IReadOnlyCollection<IGuildBehaviorRestrictionCondition> Conditions { get; }

    /// <summary>
    ///     获取限制持续时长。
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    ///     获取限制的行为类型。
    /// </summary>
    GuildBehaviorRestrictionType RestrictionType { get; }

    /// <summary>
    ///     获取此行为限制是否启用。
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    ///     获取规则创建的时间。
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     获取规则更新的时间。
    /// </summary>
    DateTimeOffset UpdatedAt { get; }
}
