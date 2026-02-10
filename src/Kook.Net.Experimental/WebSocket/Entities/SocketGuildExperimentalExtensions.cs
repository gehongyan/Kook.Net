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
        await ExperimentalClientHelper.GetBehaviorRestrictionsAsync(guild.Kook, guild.Id, options);
}
