using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     提供对 <see cref="Kook.WebSocket.SocketGuild"/> 的实验性方法。
/// </summary>
public static class SocketGuildExperimentalExtensions
{
    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.GetBehaviorRestrictionsAsync(Kook.IGuild,Kook.RequestOptions)" />
    public static async Task<IReadOnlyCollection<RestGuildBehaviorRestriction>> GetBehaviorRestrictionsAsync(
        this SocketGuild guild, RequestOptions? options = null) =>
        await ExperimentalGuildHelper.GetBehaviorRestrictionsAsync(guild.Kook, guild.Id, options);

    /// <inheritdoc cref="Kook.GuildExperimentalExtensions.CreateBehaviorRestrictionAsync(Kook.IGuild,System.String,System.Collections.Generic.IReadOnlyCollection{Kook.IGuildBehaviorRestrictionCondition},System.TimeSpan,Kook.GuildBehaviorRestrictionType,System.Boolean,Kook.RequestOptions)" />
    public static async Task<RestGuildBehaviorRestriction> CreateBehaviorRestrictionAsync(
        this SocketGuild guild, string name, IReadOnlyCollection<IGuildBehaviorRestrictionCondition> conditions,
        TimeSpan duration, GuildBehaviorRestrictionType restrictionType, bool isEnabled, RequestOptions? options) =>
        await ExperimentalGuildHelper.CreateBehaviorRestrictionAsync(
            guild.Kook, guild, name, conditions, duration, restrictionType, isEnabled, options);
}
