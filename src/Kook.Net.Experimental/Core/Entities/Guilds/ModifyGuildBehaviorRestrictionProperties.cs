namespace Kook;

/// <summary>
///     提供用于修改 <see cref="IGuildBehaviorRestriction"/> 的属性。
/// </summary>
/// <seealso cref="Kook.IGuildBehaviorRestriction.ModifyAsync(System.Action{ModifyGuildBehaviorRestrictionProperties},Kook.RequestOptions)"/>
public class ModifyGuildBehaviorRestrictionProperties
{
    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的限制条件类型。
    /// </summary>
    public IReadOnlyCollection<IGuildBehaviorRestrictionCondition>? Conditions { get; set; }

    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的限制持续时长。
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的行为类型。
    /// </summary>
    public GuildBehaviorRestrictionType? RestrictionType { get; set; }

    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的启用状态。
    /// </summary>
    public bool? IsEnabled { get; set; }
}
