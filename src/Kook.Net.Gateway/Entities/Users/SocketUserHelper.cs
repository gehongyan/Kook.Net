using System.Collections.Immutable;
using Kook.API;
using Kook.API.Rest;

namespace Kook.Gateway;

internal static class SocketUserHelper
{
    public static async Task<IReadOnlyCollection<SocketVoiceChannel>> GetConnectedChannelsAsync(IGuildUser user, BaseSocketClient client,
        RequestOptions options)
    {
        IEnumerable<Channel> channels = await client.ApiClient.GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options: options)
            .FlattenAsync().ConfigureAwait(false);
        return channels.Select(x => client.GetChannel(x.Id) as SocketVoiceChannel).ToImmutableArray();
    }

    public static async Task UpdateAsync(SocketGuildUser user, KookGatewayClient client, RequestOptions options)
    {
        GuildMember member = await client.ApiClient.GetGuildMemberAsync(user.Guild.Id, user.Id, options).ConfigureAwait(false);
        user.Update(client.State, member);
    }

    public static async Task<SocketDMChannel> CreateDMChannelAsync(SocketUser user, KookGatewayClient client, RequestOptions options)
    {
        UserChat userChat = await client.ApiClient.CreateUserChatAsync(user.Id, options).ConfigureAwait(false);
        return SocketDMChannel.Create(client, client.State, userChat.Code, userChat.Recipient);
    }
}
