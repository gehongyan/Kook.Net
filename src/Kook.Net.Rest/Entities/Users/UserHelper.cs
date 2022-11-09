using Kook.API;
using Kook.API.Rest;
using System.Collections.Immutable;

namespace Kook.Rest;

internal static class UserHelper
{
    public static async Task<string> ModifyNicknameAsync(IGuildUser user, BaseKookClient client,
        string nickname, RequestOptions options)
    {
        ModifyGuildMemberNicknameParams args = new()
        {
            GuildId = user.GuildId,
            Nickname = string.IsNullOrEmpty(nickname) ? null : nickname,
            UserId = user.Id == client.CurrentUser.Id ? null : user.Id
        };
        await client.ApiClient.ModifyGuildMemberNicknameAsync(args, options).ConfigureAwait(false);
        return nickname;
    }

    public static async Task<IReadOnlyCollection<BoostSubscriptionMetadata>> GetBoostSubscriptionsAsync(IGuildUser guildUser,
        BaseKookClient client, RequestOptions options)
    {
        IEnumerable<BoostSubscription> subscriptions = await client.ApiClient
            .GetGuildBoostSubscriptionsAsync(guildUser.GuildId, options: options).FlattenAsync();
        return subscriptions.Where(x => x.UserId == guildUser.Id)
            .GroupBy(x => (x.StartTime, x.EndTime))
            .Select(x => new BoostSubscriptionMetadata(x.Key.StartTime, x.Key.EndTime, x.Count()))
            .ToImmutableArray();
    }

    public static async Task KickAsync(IGuildUser user, BaseKookClient client, RequestOptions options)
    {
        KickOutGuildMemberParams args = new()
        {
            GuildId = user.GuildId,
            UserId = user.Id,
        };
        await client.ApiClient.KickOutGuildMemberAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<RestDMChannel> CreateDMChannelAsync(IUser user, BaseKookClient client,
        RequestOptions options)
    {
        return RestDMChannel.Create(client, await client.ApiClient.CreateUserChatAsync(user.Id, options).ConfigureAwait(false));
    }

    public static async Task AddRolesAsync(IGuildUser user, BaseKookClient client, IEnumerable<uint> roleIds, RequestOptions options)
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

    public static async Task RemoveRolesAsync(IGuildUser user, BaseKookClient client, IEnumerable<uint> roleIds, RequestOptions options)
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

    public static async Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedChannelAsync(IGuildUser user, BaseKookClient client, RequestOptions options)
    {
        var channels = await client.ApiClient.GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options: options).FlattenAsync().ConfigureAwait(false);
        return channels.Select(x => RestChannel.Create(client, x) as IVoiceChannel).ToImmutableArray();
    }

    public static async Task StartPlayingAsync(ISelfUser user, BaseKookClient client, IGame game, RequestOptions options)
    {
        await client.ApiClient.BeginActivityAsync(new BeginActivityParams(ActivityType.Game) { Id = game.Id }, options).ConfigureAwait(false);
    }

    public static async Task StartPlayingAsync(ISelfUser user, BaseKookClient client, Music music, RequestOptions options)
    {
        await client.ApiClient.BeginActivityAsync(new BeginActivityParams(ActivityType.Music) { MusicProvider = music.Provider, MusicName = music.Name, Signer = music.Singer }, options).ConfigureAwait(false);
    }

    public static async Task StopPlayingAsync(ISelfUser user, BaseKookClient client, ActivityType type, RequestOptions options)
    {
        await client.ApiClient.EndActivityAsync(new EndGameActivityParams(type), options: options).ConfigureAwait(false);
    }

    public static async Task<RestIntimacy> GetIntimacyAsync(IUser user, BaseKookClient client, RequestOptions options)
    {
        Intimacy intimacy = await client.ApiClient.GetIntimacyAsync(user.Id, options: options).ConfigureAwait(false);
        return RestIntimacy.Create(client, user, intimacy);
    }

    public static async Task UpdateIntimacyAsync(IUser user, BaseKookClient client, Action<IntimacyProperties> func, RequestOptions options)
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