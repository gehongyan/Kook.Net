using System.Collections.Immutable;
using KaiHeiLa.API;
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
    
    public static async Task<RestDMChannel> CreateDMChannelAsync(IUser user, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        return RestDMChannel.Create(client, await client.ApiClient.CreateUserChatAsync(user.Id, options).ConfigureAwait(false));
    }
    
    public static async Task AddRolesAsync(IGuildUser user, BaseKaiHeiLaClient client, IEnumerable<uint> roleIds, RequestOptions options)
    {
        var args = roleIds.Select(x => new AddOrRemoveRoleParams()
        {
            GuildId = user.GuildId,
            RoleId = x,
            UserId = user.Id
        });
        foreach (var arg in args)
            await client.ApiClient.AddRoleAsync(arg, options).ConfigureAwait(false);
    }

    public static async Task RemoveRolesAsync(IGuildUser user, BaseKaiHeiLaClient client, IEnumerable<uint> roleIds, RequestOptions options)
    {
        var args = roleIds.Select(x => new AddOrRemoveRoleParams()
        {
            GuildId = user.GuildId,
            RoleId = x,
            UserId = user.Id
        });
        foreach (var arg in args)
            await client.ApiClient.RemoveRoleAsync(arg, options).ConfigureAwait(false);
    }
    
    public static async Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedChannelAsync(IGuildUser user, BaseKaiHeiLaClient client, RequestOptions options)
    {
        var channels = await client.ApiClient.GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options: options).FlattenAsync().ConfigureAwait(false);
        return channels.Select(x => RestChannel.Create(client, x) as IVoiceChannel).ToImmutableArray();
    }
    
    public static async Task StartPlayingAsync(ISelfUser user, BaseKaiHeiLaClient client, IGame game, RequestOptions options)
    { 
        await client.ApiClient.BeginGameActivityAsync(game.Id, options).ConfigureAwait(false);
    }
    
    public static async Task StopPlayingAsync(ISelfUser user, BaseKaiHeiLaClient client, RequestOptions options)
    { 
        await client.ApiClient.EndGameActivityAsync(options: options).ConfigureAwait(false);
    }
    
    public static async Task<RestIntimacy> GetIntimacyAsync(IUser user, BaseKaiHeiLaClient client, RequestOptions options)
    {
        Intimacy intimacy = await client.ApiClient.GetIntimacyAsync(user.Id, options: options).ConfigureAwait(false);
        return RestIntimacy.Create(client, user, intimacy);
    }
    
    public static async Task UpdateIntimacyAsync(IUser user, BaseKaiHeiLaClient client, Action<IntimacyProperties> func, RequestOptions options)
    {
        IntimacyProperties properties = new();
        func(properties);
        var args = new UpdateIntimacyValueParams()
        {
            UserId = user.Id,
            Score = properties.Score,
            SocialInfo = properties.SocialInfo,
            ImageId = properties.ImageId
        };
        await client.ApiClient.UpdateIntimacyValueAsync(args, options).ConfigureAwait(false);
    }
        
}