using System.Collections.Immutable;

namespace KaiHeiLa.WebSocket;

internal static class SocketUserHelper
{
    public static async Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedChannelAsync(IGuildUser user, BaseSocketClient client, RequestOptions options)
    {
        var channels = await client.ApiClient.GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options).ConfigureAwait(false);
        return channels.Select(x => client.GetChannel(x.Id) as IVoiceChannel).ToImmutableArray();
    }
}