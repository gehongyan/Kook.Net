namespace Kook;

/// <summary>
///     表示一个服务器严重违规记录行为限制。
/// </summary>
public readonly struct GuildViolationBehaviorRestrictionCondition : IGuildBehaviorRestrictionCondition
{
    /// <inheritdoc />
    public GuildRestrictionConditionType Type => GuildRestrictionConditionType.Violation;
}