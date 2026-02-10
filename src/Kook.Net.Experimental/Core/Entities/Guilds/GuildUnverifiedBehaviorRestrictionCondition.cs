namespace Kook;

/// <summary>
///     表示一个服务器非实名用户行为限制。
/// </summary>
public readonly struct GuildUnverifiedBehaviorRestrictionCondition : IGuildBehaviorRestrictionCondition
{
    /// <inheritdoc />
    public GuildRestrictionConditionType Type => GuildRestrictionConditionType.Unverified;
}