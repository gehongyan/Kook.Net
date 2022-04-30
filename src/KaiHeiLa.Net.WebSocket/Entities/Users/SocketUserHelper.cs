using System.Collections.Immutable;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.WebSocket;

internal static class SocketUserHelper
{
    public static async Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedChannelsAsync(IGuildUser user, BaseSocketClient client, RequestOptions options)
    {
        var channels = await client.ApiClient.GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options: options).FlattenAsync().ConfigureAwait(false);
        return channels.Select(x => client.GetChannel(x.Id) as IVoiceChannel).ToImmutableArray();
    }

    public static async Task ReloadAsync(SocketGuildUser user, KaiHeiLaSocketClient client, RequestOptions options)
    {
        GuildMember member = await client.ApiClient.GetGuildMemberAsync(user.Guild.Id, user.Id, options: options).ConfigureAwait(false);
        user.Update(client.State, member);
    }
}