using System.Collections.Immutable;
using Kook.API;
using Kook.API.Rest;

namespace Kook.Rest;

internal static class UserHelper
{
    public static async Task<string?> ModifyNicknameAsync(IGuildUser user, BaseKookClient client,
        string? nickname, RequestOptions? options)
    {
        ModifyGuildMemberNicknameParams args = new()
        {
            GuildId = user.GuildId,
            Nickname = nickname,
            UserId = user.Id == client.CurrentUser?.Id ? null : user.Id
        };
        await client.ApiClient.ModifyGuildMemberNicknameAsync(args, options).ConfigureAwait(false);
        return nickname;
    }

    public static async Task<IReadOnlyCollection<BoostSubscriptionMetadata>> GetBoostSubscriptionsAsync(
        IGuildUser guildUser, BaseKookClient client, RequestOptions? options)
    {
        IEnumerable<BoostSubscription> subscriptions = await client.ApiClient
            .GetGuildBoostSubscriptionsAsync(guildUser.GuildId, options: options).FlattenAsync();
        return subscriptions.Where(x => x.UserId == guildUser.Id)
            .GroupBy(x => (x.StartTime, x.EndTime))
            .Select(x =>
                new BoostSubscriptionMetadata(x.Key.StartTime, x.Key.EndTime, x.Count()))
            .ToImmutableArray();
    }

    public static async Task KickAsync(IGuildUser user, BaseKookClient client, RequestOptions? options)
    {
        KickOutGuildMemberParams args = new() { GuildId = user.GuildId, UserId = user.Id };
        await client.ApiClient.KickOutGuildMemberAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<RestDMChannel> CreateDMChannelAsync(IUser user,
        BaseKookClient client, RequestOptions? options) =>
        RestDMChannel.Create(client, await client.ApiClient
            .CreateUserChatAsync(user.Id, options).ConfigureAwait(false));

    public static async Task AddRolesAsync(RestGuildUser user, BaseKookClient client,
        IEnumerable<uint> roleIds, RequestOptions? options)
    {
        IEnumerable<AddOrRemoveRoleParams> args = roleIds
            .Distinct()
            .Select(x => new AddOrRemoveRoleParams
            {
                GuildId = user.GuildId,
                RoleId = x, UserId = user.Id
            });
        foreach (AddOrRemoveRoleParams arg in args)
        {
            await client.ApiClient.AddRoleAsync(arg, options).ConfigureAwait(false);
            user.AddRole(arg.RoleId);
        }
    }

    public static async Task RemoveRolesAsync(RestGuildUser user, BaseKookClient client,
        IEnumerable<uint> roleIds, RequestOptions? options)
    {
        IEnumerable<AddOrRemoveRoleParams> args = roleIds
            .Distinct()
            .Select(x => new AddOrRemoveRoleParams
            {
                GuildId = user.GuildId,
                RoleId = x, UserId = user.Id
            });
        foreach (AddOrRemoveRoleParams arg in args)
        {
            await client.ApiClient.RemoveRoleAsync(arg, options).ConfigureAwait(false);
            user.RemoveRole(arg.RoleId);
        }
    }

    public static async Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedChannelAsync(
        IGuildUser user, BaseKookClient client, RequestOptions? options)
    {
        IEnumerable<Channel> channels = await client.ApiClient
            .GetAudioChannelsUserConnectsAsync(user.GuildId, user.Id, options: options)
            .FlattenAsync().ConfigureAwait(false);
        return [..channels.Select(x => RestChannel.Create(client, x)).Cast<IVoiceChannel>()];
    }

    public static async Task StartPlayingAsync(ISelfUser user,
        BaseKookClient client, IGame game, RequestOptions? options)
    {
        BeginActivityParams args = new()
        {
            ActivityType = ActivityType.Game,
            Id = game.Id
        };
        await client.ApiClient.BeginActivityAsync(args, options).ConfigureAwait(false);
    }

    public static async Task StartPlayingAsync(ISelfUser user,
        BaseKookClient client, Music music, RequestOptions? options)
    {
        BeginActivityParams args = new()
        {
            ActivityType = ActivityType.Music,
            MusicProvider = music.Provider,
            MusicName = music.Name,
            Signer = music.Singer
        };
        await client.ApiClient.BeginActivityAsync(args, options).ConfigureAwait(false);
    }

    public static async Task StopPlayingAsync(ISelfUser user,
        BaseKookClient client, ActivityType type, RequestOptions? options)
    {
        EndGameActivityParams args = new()
        {
            ActivityType = type
        };
        await client.ApiClient.EndActivityAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<RestIntimacy> GetIntimacyAsync(IUser user, BaseKookClient client, RequestOptions? options)
    {
        Intimacy intimacy = await client.ApiClient.GetIntimacyAsync(user.Id, options).ConfigureAwait(false);
        return RestIntimacy.Create(client, user, intimacy);
    }

    public static async Task UpdateIntimacyAsync(IUser user, BaseKookClient client,
        Action<IntimacyProperties> func, RequestOptions? options)
    {
        RestIntimacy intimacy = await GetIntimacyAsync(user, client, options).ConfigureAwait(false);
        IntimacyProperties properties = new(intimacy.SocialInfo, intimacy.Score);
        func(properties);
        UpdateIntimacyValueParams args = new()
        {
            UserId = user.Id,
            Score = properties.Score,
            SocialInfo = properties.SocialInfo,
            ImageId = properties.ImageId
        };
        await client.ApiClient.UpdateIntimacyValueAsync(args, options).ConfigureAwait(false);
    }

    public static async Task BlockAsync(IUser user, BaseKookClient client, RequestOptions? options)
    {
        BlockUserParams args = new()
        {
            UserId = user.Id
        };
        await client.ApiClient.BlockUserAsync(args, options).ConfigureAwait(false);
    }

    public static async Task UnblockAsync(IUser user, BaseKookClient client, RequestOptions? options)
    {
        UnblockUserParams args = new()
        {
            UserId = user.Id
        };
        await client.ApiClient.UnblockUserAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RequestFriendAsync(IUser user, BaseKookClient client, RequestOptions? options)
    {
        RequestFriendParams args = new()
        {
            FullQualification = user.UsernameAndIdentifyNumber(false),
            Source = RequestFriendSource.FullQualification
        };
        await client.ApiClient.RequestFriendAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RequestFriendAsync(IGuildUser user, BaseKookClient client, RequestOptions? options)
    {
        RequestFriendParams args = new()
        {
            FullQualification = user.UsernameAndIdentifyNumber(false),
            Source = RequestFriendSource.Guild, GuildId = user.GuildId
        };
        await client.ApiClient.RequestFriendAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RequestIntimacyRelationAsync(IUser user,
        IntimacyRelationType relationType, BaseKookClient client, RequestOptions? options)
    {
        RequestIntimacyParams args = new()
        {
            FullQualification = user.UsernameAndIdentifyNumber(false),
            RelationType = relationType
        };
        await client.ApiClient.RequestIntimacyAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemoveFriendAsync(IUser user, BaseKookClient client, RequestOptions? options)
    {
        RemoveFriendParams args = new()
        {
            UserId = user.Id
        };
        await client.ApiClient.RemoveFriendAsync(args, options).ConfigureAwait(false);
    }

    public static async Task UnravelIntimacyRelationAsync(IUser user,
        bool removeFriend, BaseKookClient client, RequestOptions? options)
    {
        UnravelRelationParams args = new()
        {
            UserId = user.Id,
            RemoveFriend = removeFriend
        };
        await client.ApiClient.UnravelIntimacyAsync(args, options).ConfigureAwait(false);
    }

    public static async Task HandleFriendRequestAsync(IFriendRequest request,
        bool handleResult, BaseKookClient client, RequestOptions? options)
    {
        HandleFriendRequestParams args = new()
        {
            Id = request.Id,
            HandleResult = handleResult
        };
        await client.ApiClient.HandleFriendRequestAsync(args, options).ConfigureAwait(false);
    }

    public static async Task HandleIntimacyRequestAsync(IIntimacyRelationRequest request,
        bool handleResult, BaseKookClient client, RequestOptions? options)
    {
        HandleFriendRequestParams args = new()
        {
            Id = request.Id,
            HandleResult = handleResult
        };
        await client.ApiClient.HandleIntimacyRequestAsync(args, options).ConfigureAwait(false);
    }
}
