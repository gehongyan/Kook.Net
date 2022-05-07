using System.Collections.Immutable;
using KaiHeiLa.API;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.WebSocket;

internal static class SocketUserHelper
{
    public static async Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedChannelsAsync(IGuildUser user, BaseSocketClient client, RequestOptions options)
    {
        var channels = await client.ApiClient.GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options: options).FlattenAsync().ConfigureAwait(false);
        return channels.Select(x => client.GetChannel(x.Id) as IVoiceChannel).ToImmutableArray();
    }

    public static async Task UpdateAsync(SocketGuildUser user, KaiHeiLaSocketClient client, RequestOptions options)
    {
        GuildMember member = await client.ApiClient.GetGuildMemberAsync(user.Guild.Id, user.Id, options: options).ConfigureAwait(false);
        user.Update(client.State, member);
    }

    public static async Task<SocketDMChannel> CreateDMChannelAsync(SocketUser user, KaiHeiLaSocketClient client, RequestOptions options)
    {
        UserChat userChat = await client.ApiClient.CreateUserChatAsync(user.Id, options).ConfigureAwait(false);
        return SocketDMChannel.Create(client,client.State, userChat.Code, userChat.Recipient);
    }
}