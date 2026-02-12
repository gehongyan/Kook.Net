namespace Kook;

/// <summary>
///     表示一个服务器非实名用户行为限制。
/// </summary>
public readonly struct UnverifiedBehaviorRestrictionCondition : IBehaviorRestrictionCondition
{
    /// <inheritdoc />
    public RestrictionConditionType Type => RestrictionConditionType.Unverified;
}
