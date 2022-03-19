using System.Collections.Immutable;
using KaiHeiLa.API.Rest;
using Model = KaiHeiLa.API.User;

namespace KaiHeiLa.Rest;

internal static class UserHelper
{
    public static async Task<string> ModifyNicknameAsync(IGuildUser user, BaseKaiHeiLaClient client,
        Action<string> func, RequestOptions options)
    {
        var nickname = user.Nickname;
        func(nickname);

        ModifyGuildMemberNicknameParams args = new()
        {
            GuildId = user.GuildId,
            Nickname = string.IsNullOrEmpty(nickname) ? null : nickname,
            UserId = user.Id == client.CurrentUser.Id ? null : user.Id
        };
        await client.ApiClient.ModifyGuildMemberNicknameAsync(args, options).ConfigureAwait(false);
        return nickname;
    }

    public static async Task KickAsync(IGuildUser user, BaseKaiHeiLaClient client, RequestOptions options)
    {
        KickOutGuildMemberParams args = new()
        {
            GuildId = user.GuildId,
            UserId = user.Id,
        };
        await client.ApiClient.KickOutGuildMemberAsync(args, options).ConfigureAwait(false);
    }
    
    public static async Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedChannelAsync(IGuildUser user, BaseKaiHeiLaClient client, RequestOptions options)
    {
        var channels = await client.ApiClient.GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options).ConfigureAwait(false);
        return channels.Select(x => RestChannel.Create(client, x) as IVoiceChannel).ToImmutableArray();
    }
}