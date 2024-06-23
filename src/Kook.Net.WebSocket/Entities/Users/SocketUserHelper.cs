using Kook.API;
using Kook.API.Rest;
using System.Collections.Immutable;

namespace Kook.WebSocket;

internal static class SocketUserHelper
{
    public static async Task<IReadOnlyCollection<SocketVoiceChannel>> GetConnectedChannelsAsync(IGuildUser user, BaseSocketClient client,
        RequestOptions? options)
    {
        IEnumerable<Channel> channels = await client.ApiClient.GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options: options)
            .FlattenAsync().ConfigureAwait(false);
        return channels.Select(x => client.GetChannel(x.Id) as SocketVoiceChannel).ToImmutableArray();
    }

    public static async Task UpdateAsync(SocketGuildUser user, KookSocketClient client, RequestOptions? options)
    {
        GuildMember member = await client.ApiClient.GetGuildMemberAsync(user.Guild.Id, user.Id, options).ConfigureAwait(false);
        user.Update(client.State, member);
    }

    public static async Task UpdateAsync(SocketSelfUser user, KookSocketClient client, RequestOptions? options)
    {
        SelfUser selfUser = await client.ApiClient.GetSelfUserAsync(options).ConfigureAwait(false);
        user.Update(client.State, selfUser);
    }

    public static async Task<SocketDMChannel> CreateDMChannelAsync(SocketUser user, KookSocketClient client, RequestOptions? options)
    {
        UserChat userChat = await client.ApiClient.CreateUserChatAsync(user.Id, options).ConfigureAwait(false);
        return SocketDMChannel.Create(client, client.State, userChat.Code, userChat.Recipient);
    }

    public static async Task AddRolesAsync(SocketGuildUser user, KookSocketClient client,
        IEnumerable<uint> roleIds, RequestOptions? options)
    {
        IEnumerable<AddOrRemoveRoleParams> args = roleIds
            .Distinct()
            .Select(x => new AddOrRemoveRoleParams
            {
                GuildId = user.Guild.Id,
                RoleId = x, UserId = user.Id
            });
        foreach (AddOrRemoveRoleParams arg in args)
        {
            await client.ApiClient.AddRoleAsync(arg, options).ConfigureAwait(false);
            user.AddRole(arg.RoleId);
        }
    }

    public static async Task RemoveRolesAsync(SocketGuildUser user, KookSocketClient client,
        IEnumerable<uint> roleIds, RequestOptions? options)
    {
        IEnumerable<AddOrRemoveRoleParams> args = roleIds
            .Distinct()
            .Select(x => new AddOrRemoveRoleParams
            {
                GuildId = user.Guild.Id,
                RoleId = x, UserId = user.Id
            });
        foreach (AddOrRemoveRoleParams arg in args)
        {
            await client.ApiClient.RemoveRoleAsync(arg, options).ConfigureAwait(false);
            user.RemoveRole(arg.RoleId);
        }
    }

}
