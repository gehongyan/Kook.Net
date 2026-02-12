namespace Kook;

/// <summary>
///     表示一个服务器非大陆用户行为限制。
/// </summary>
public readonly struct OverseasBehaviorRestrictionCondition : IBehaviorRestrictionCondition
{
    /// <inheritdoc />
    public RestrictionConditionType Type => RestrictionConditionType.Overseas;
}
