using System.Collections.Immutable;
using Kook.API;
using Kook.API.Rest;

namespace Kook.Rest;

internal static class ClientHelper
{
    public static async Task<RestGuild> GetGuildAsync(BaseKookClient client,
        ulong id, RequestOptions options)
    {
        ExtendedGuild model = await client.ApiClient.GetGuildAsync(id, options).ConfigureAwait(false);
        if (model != null) return RestGuild.Create(client, model);

        return null;
    }

    public static async Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(BaseKookClient client, RequestOptions options)
    {
        ImmutableArray<RestGuild>.Builder guilds = ImmutableArray.CreateBuilder<RestGuild>();
        if (client.TokenType is TokenType.Bot)
        {
            IReadOnlyCollection<RichGuild> models = await client.ApiClient.ListGuildsAsync(options).ConfigureAwait(false);
            foreach (RichGuild model in models) guilds.Add(RestGuild.Create(client, model));
        }
        else
        {
            IEnumerable<Guild> models = await client.ApiClient.GetGuildsAsync(options: options).FlattenAsync().ConfigureAwait(false);
            foreach (Guild model in models) guilds.Add(RestGuild.Create(client, model));
        }

        return guilds.ToImmutable();
    }

    public static async Task<RestChannel> GetChannelAsync(BaseKookClient client,
        ulong id, RequestOptions options)
    {
        Channel model = await client.ApiClient.GetGuildChannelAsync(id, options).ConfigureAwait(false);
        if (model != null) return RestChannel.Create(client, model);

        return null;
    }

    public static async Task<RestDMChannel> GetDMChannelAsync(BaseKookClient client,
        Guid chatCode, RequestOptions options)
    {
        UserChat model = await client.ApiClient.GetUserChatAsync(chatCode, options).ConfigureAwait(false);
        if (model != null) return RestDMChannel.Create(client, model);

        return null;
    }

    public static async Task<IReadOnlyCollection<RestDMChannel>> GetDMChannelsAsync(BaseKookClient client, RequestOptions options)
    {
        IEnumerable<UserChat> model = await client.ApiClient.GetUserChatsAsync(options: options).FlattenAsync().ConfigureAwait(false);
        if (model != null) return model.Select(x => RestDMChannel.Create(client, x)).ToImmutableArray();

        return null;
    }

    public static async Task<RestUser> GetUserAsync(BaseKookClient client,
        ulong id, RequestOptions options)
    {
        User model = await client.ApiClient.GetUserAsync(id, options).ConfigureAwait(false);
        if (model != null) return RestUser.Create(client, model);

        return null;
    }

    public static async Task<RestGuildUser> GetGuildMemberAsync(BaseKookClient client,
        ulong guildId, ulong id, RequestOptions options)
    {
        RestGuild guild = await GetGuildAsync(client, guildId, options).ConfigureAwait(false);
        if (guild == null) return null;

        GuildMember model = await client.ApiClient.GetGuildMemberAsync(guildId, id, options).ConfigureAwait(false);
        if (model != null) return RestGuildUser.Create(client, guild, model);

        return null;
    }

    public static async Task MoveUsersAsync(BaseKookClient client, IEnumerable<IGuildUser> userIds, IVoiceChannel targetChannel,
        RequestOptions options)
    {
        MoveUsersParams args = new() { ChannelId = targetChannel.Id, UserIds = userIds.Select(x => x.Id).ToArray() };
        await client.ApiClient.MoveUsersAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<IReadOnlyCollection<RestUser>> GetFriendsAsync(BaseKookClient client, RequestOptions options)
    {
        GetFriendStatesResponse models = await client.ApiClient.GetFriendStatesAsync(FriendState.Accepted, options).ConfigureAwait(false);
        if (models != null) return models.Friends.Select(x => RestUser.Create(client, x.User)).ToImmutableArray();

        return null;
    }

    // public static async Task RequestFriendAsync(BaseKookClient client, string username, ushort identifyNumberValue, RequestOptions options)
    // {
    //     // TODO: Add a better way to validate the identify number.
    //     Preconditions.AtMost(identifyNumberValue, (ushort)9999, nameof(identifyNumberValue));
    //     Preconditions.AtLeast(identifyNumberValue, (ushort)1, nameof(identifyNumberValue));
    //     RequestFriendParams args = new() { FullQualification = $"{username}#{identifyNumberValue:D4}", Source = RequestFriendSource.FullQualification };
    //     await client.ApiClient.RequestFriendAsync(args, options).ConfigureAwait(false);
    // }

    public static async Task<IReadOnlyCollection<RestFriendRequest>> GetFriendRequestsAsync(BaseKookClient client, RequestOptions options)
    {
        GetFriendStatesResponse models = await client.ApiClient.GetFriendStatesAsync(FriendState.Pending, options).ConfigureAwait(false);
        if (models != null) return models.FriendRequests.Select(x => RestFriendRequest.Create(client, x)).ToImmutableArray();

        return null;
    }

    public static async Task<IReadOnlyCollection<RestUser>> GetBlockedUsersAsync(BaseKookClient client, RequestOptions options)
    {
        GetFriendStatesResponse models = await client.ApiClient.GetFriendStatesAsync(FriendState.Blocked, options).ConfigureAwait(false);
        if (models != null) return models.BlockedUsers.Select(x => RestUser.Create(client, x.User)).ToImmutableArray();

        return null;
    }

    public static async Task<string> CreateAssetAsync(BaseKookClient client, Stream stream, string fileName, RequestOptions options)
    {
        CreateAssetResponse model = await client.ApiClient.CreateAssetAsync(new CreateAssetParams { File = stream, FileName = fileName }, options);
        if (model != null) return model.Url;

        return null;
    }

    public static IAsyncEnumerable<IReadOnlyCollection<RestGame>> GetGamesAsync(BaseKookClient client, GameCreationSource? source,
        RequestOptions options) =>
        client.ApiClient.GetGamesAsync(source, options: options)
            .Select(x => x.Select(y => RestGame.Create(client, y)).ToImmutableArray() as IReadOnlyCollection<RestGame>);

    public static async Task<RestGame> CreateGameAsync(BaseKookClient client, string name, string processName, string iconUrl, RequestOptions options)
    {
        CreateGameParams args = new() { Icon = iconUrl, Name = name, ProcessName = processName };
        Game model = await client.ApiClient.CreateGameAsync(args, options).ConfigureAwait(false);
        return RestGame.Create(client, model);
    }

    public static async Task DeleteGameAsync(BaseKookClient client, int id, RequestOptions options) =>
        await client.ApiClient.DeleteGameAsync(id, options).ConfigureAwait(false);

    public static async Task AddRoleAsync(BaseKookClient client, ulong guildId, ulong userId, uint roleId, RequestOptions options = null)
    {
        AddOrRemoveRoleParams args = new() { GuildId = guildId, RoleId = roleId, UserId = userId };
        await client.ApiClient.AddRoleAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemoveRoleAsync(BaseKookClient client, ulong guildId, ulong userId, uint roleId, RequestOptions options = null)
    {
        AddOrRemoveRoleParams args = new() { GuildId = guildId, RoleId = roleId, UserId = userId };
        await client.ApiClient.RemoveRoleAsync(args, options).ConfigureAwait(false);
    }
}
