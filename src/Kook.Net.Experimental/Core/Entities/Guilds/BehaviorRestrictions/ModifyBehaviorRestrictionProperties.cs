namespace Kook;

/// <summary>
///     提供用于修改 <see cref="IBehaviorRestriction"/> 的属性。
/// </summary>
/// <seealso cref="Kook.IBehaviorRestriction.ModifyAsync(System.Action{Kook.ModifyBehaviorRestrictionProperties},Kook.RequestOptions)"/>
public class ModifyBehaviorRestrictionProperties
{
    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的限制条件类型。
    /// </summary>
    public IReadOnlyCollection<IBehaviorRestrictionCondition>? Conditions { get; set; }

    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的限制持续时长。
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的行为类型。
    /// </summary>
    public BehaviorRestrictionType? RestrictionType { get; set; }

    /// <summary>
    ///     获取或设置要设置到此服务器行为限制的启用状态。
    /// </summary>
    public bool? IsEnabled { get; set; }
}
