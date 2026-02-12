using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     提供对 <see cref="Kook.WebSocket.SocketGuild"/> 的实验性方法。
/// </summary>
public static class SocketGuildExperimentalExtensions
{
    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.GetBehaviorRestrictionsAsync(Kook.IGuild,Kook.RequestOptions)" />
    public static async Task<IReadOnlyCollection<RestBehaviorRestriction>> GetBehaviorRestrictionsAsync(
        this SocketGuild guild, RequestOptions? options = null) =>
        await ExperimentalGuildHelper.GetBehaviorRestrictionsAsync(guild.Kook, guild.Id, options);

    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.CreateBehaviorRestrictionAsync(Kook.IGuild,System.String,System.Collections.Generic.IReadOnlyCollection{IBehaviorRestrictionCondition},System.TimeSpan,BehaviorRestrictionType,System.Boolean,Kook.RequestOptions)" />
    public static async Task<RestBehaviorRestriction> CreateBehaviorRestrictionAsync(
        this SocketGuild guild, string name, IReadOnlyCollection<IBehaviorRestrictionCondition> conditions,
        TimeSpan duration, BehaviorRestrictionType restrictionType, bool isEnabled = true, RequestOptions? options = null) =>
        await ExperimentalGuildHelper.CreateBehaviorRestrictionAsync(
            guild.Kook, guild, name, conditions, duration, restrictionType, isEnabled, options);

    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.GetContentFiltersAsync(Kook.IGuild,Kook.RequestOptions)" />
    public static async Task<IReadOnlyCollection<RestContentFilter>> GetContentFiltersAsync(
        this SocketGuild guild, RequestOptions? options = null) =>
        await ExperimentalGuildHelper.GetContentFiltersAsync(guild.Kook, guild.Id, options);

    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.CreateContentFilterAsync(Kook.IGuild,Kook.IContentFilterTarget,System.Collections.Generic.IReadOnlyCollection{Kook.IContentFilterHandler},System.Collections.Generic.IReadOnlyCollection{Kook.ContentFilterExemption},System.Boolean,Kook.RequestOptions)" />
    public static async Task<RestContentFilter> CreateContentFilterAsync(
        this SocketGuild guild, IContentFilterTarget target, IReadOnlyCollection<IContentFilterHandler>? handlers = null,
        IReadOnlyCollection<ContentFilterExemption>? exemptions = null, bool isEnabled = true, RequestOptions? options = null) =>
        await ExperimentalGuildHelper.CreateContentFilterAsync(
            guild.Kook, guild, target, handlers, exemptions, isEnabled, options);
}
