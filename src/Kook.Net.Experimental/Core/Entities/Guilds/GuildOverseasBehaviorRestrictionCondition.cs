namespace Kook;

/// <summary>
///     表示一个服务器非大陆用户行为限制。
/// </summary>
public readonly struct GuildOverseasBehaviorRestrictionCondition : IGuildBehaviorRestrictionCondition
{
    /// <inheritdoc />
    public GuildRestrictionConditionType Type => GuildRestrictionConditionType.Overseas;
}
