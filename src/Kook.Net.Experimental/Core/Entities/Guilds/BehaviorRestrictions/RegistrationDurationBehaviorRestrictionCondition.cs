namespace Kook;

/// <summary>
///     表示一个服务器注册时长行为限制。
/// </summary>
public readonly struct RegistrationDurationBehaviorRestrictionCondition : IBehaviorRestrictionCondition
{
    /// <inheritdoc />
    public RestrictionConditionType Type => RestrictionConditionType.RegistrationDuration;

    /// <summary>
    ///     获取限制条件要求的最短注册时长。
    /// </summary>
    public TimeSpan MinimumDuration { get; init; }
}
