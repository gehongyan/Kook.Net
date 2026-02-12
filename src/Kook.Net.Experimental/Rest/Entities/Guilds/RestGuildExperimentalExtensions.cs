namespace Kook.Rest;

/// <summary>
///     提供对 <see cref="Kook.Rest.RestGuild"/> 的实验性方法。
/// </summary>
public static class RestGuildExperimentalExtensions
{
    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.GetBehaviorRestrictionsAsync(Kook.IGuild,Kook.RequestOptions)" />
    public static async Task<IReadOnlyCollection<RestBehaviorRestriction>> GetBehaviorRestrictionsAsync(
        this RestGuild guild, RequestOptions? options = null) =>
        await ExperimentalGuildHelper.GetBehaviorRestrictionsAsync(guild.Kook, guild.Id, options);

    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.CreateBehaviorRestrictionAsync(Kook.IGuild,System.String,System.Collections.Generic.IReadOnlyCollection{IBehaviorRestrictionCondition},System.TimeSpan,BehaviorRestrictionType,System.Boolean,Kook.RequestOptions)" />
    public static async Task<RestBehaviorRestriction> CreateBehaviorRestrictionAsync(
        this RestGuild guild, string name, IReadOnlyCollection<IBehaviorRestrictionCondition> conditions,
        TimeSpan duration, BehaviorRestrictionType restrictionType, bool isEnabled, RequestOptions? options) =>
        await ExperimentalGuildHelper.CreateBehaviorRestrictionAsync(
            guild.Kook, guild, name, conditions, duration, restrictionType, isEnabled, options);
}
