using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based KOOK client.
/// </summary>
public class KookRestClient : BaseKookClient, IKookClient
{
    #region KookRestClient

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    ///     Gets the logged-in user.
    /// </summary>
    public new RestSelfUser? CurrentUser
    {
        get => base.CurrentUser as RestSelfUser;
        internal set => base.CurrentUser = value;
    }

    /// <summary>
    ///     Initializes a new REST-based KOOK client with the default configuration.
    /// </summary>
    public KookRestClient()
        : this(new KookRestConfig())
    {
    }

    /// <summary>
    ///     Initializes a new REST-based KOOK client with the specified configuration.
    /// </summary>
    /// <param name="config"> The configuration to use. </param>
    public KookRestClient(KookRestConfig config)
        : base(config, CreateApiClient(config))
    {
    }

    internal KookRestClient(KookRestConfig config, API.KookRestApiClient api)
        : base(config, api)
    {
    }

    private static API.KookRestApiClient CreateApiClient(KookRestConfig config) =>
        new(config.RestClientProvider, KookConfig.UserAgent, config.AcceptLanguage,
            config.DefaultRetryMode, SerializerOptions);

    internal override void Dispose(bool disposing)
    {
        if (disposing) ApiClient.Dispose();
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    internal override async Task OnLoginAsync(TokenType tokenType, string token)
    {
        RequestOptions requestOptions = new() { RetryMode = RetryMode.AlwaysRetry };
        SelfUser user = await ApiClient.GetSelfUserAsync(requestOptions).ConfigureAwait(false);
        ApiClient.CurrentUserId = user.Id;
        base.CurrentUser = RestSelfUser.Create(this, user);
    }

    internal void CreateRestSelfUser(SelfUser user) => base.CurrentUser = RestSelfUser.Create(this, user);

    /// <inheritdoc />
    internal override Task OnLogoutAsync() => Task.CompletedTask;

    #endregion

    #region Guilds

    /// <summary>
    ///     Gets a guild.
    /// </summary>
    /// <param name="id">The guild identifier.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the guild associated
    ///     with the identifier; <c>null</c> when the guild cannot be found.
    /// </returns>
    public Task<RestGuild> GetGuildAsync(ulong id, RequestOptions? options = null) =>
        ClientHelper.GetGuildAsync(this, id, options);

    /// <summary>
    ///     Gets a collection of guilds that the user is currently in.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
    ///     of guilds that the current user is in.
    /// </returns>
    public Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(RequestOptions? options = null) =>
        ClientHelper.GetGuildsAsync(this, options);

    #endregion

    #region Channels

    /// <summary>
    ///     Gets a generic channel.
    /// </summary>
    /// <param name="id">The identifier of the channel.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public Task<RestChannel> GetChannelAsync(ulong id, RequestOptions? options = null) =>
        ClientHelper.GetChannelAsync(this, id, options);

    /// <summary>
    ///     Gets a direct message channel.
    /// </summary>
    /// <param name="chatCode">The identifier of the channel.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
    ///     of direct-message channels that the user currently partakes in.
    /// </returns>
    public Task<RestDMChannel> GetDMChannelAsync(Guid chatCode, RequestOptions? options = null) =>
        ClientHelper.GetDMChannelAsync(this, chatCode, options);

    /// <summary>
    ///     Gets a collection of direct message channels opened in this session.
    /// </summary>
    /// <remarks>
    ///     This method returns a collection of currently opened direct message channels.
    ///     <note type="warning">
    ///         This method will not return previously opened DM channels outside of the current session! If you
    ///         have just started the client, this may return an empty collection.
    ///     </note>
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
    ///     of direct-message channels that the user currently partakes in.
    /// </returns>
    public Task<IReadOnlyCollection<RestDMChannel>> GetDMChannelsAsync(RequestOptions? options = null) =>
        ClientHelper.GetDMChannelsAsync(this, options);

    #endregion

    #region Roles

    /// <summary>
    ///     Adds the specified role to this user in the guild.
    /// </summary>
    /// <param name="guildId"> The guild where the role and user are located. </param>
    /// <param name="userId"> The user to add the role to. </param>
    /// <param name="roleId"> The role to be added to the user. </param>
    /// <returns>
    ///     A task that represents the asynchronous role addition operation.
    /// </returns>
    public Task AddRoleAsync(ulong guildId, ulong userId, uint roleId) =>
        ClientHelper.AddRoleAsync(this, guildId, userId, roleId);

    /// <summary>
    ///     Removes the specified <paramref name="roleId"/> from this user in the guild.
    /// </summary>
    /// <param name="guildId"> The guild where the role and user are located. </param>
    /// <param name="userId"> The user to remove the role from. </param>
    /// <param name="roleId"> The role to be removed from the user. </param>
    /// <returns>
    ///     A task that represents the asynchronous role removal operation.
    /// </returns>
    public Task RemoveRoleAsync(ulong guildId, ulong userId, uint roleId) =>
        ClientHelper.RemoveRoleAsync(this, guildId, userId, roleId);

    #endregion

    #region Users

    /// <summary>
    ///     Gets a user.
    /// </summary>
    /// <param name="id"> The identifier of the user (e.g. `168693960628371456`). </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the user associated with
    ///     the identifier; <c>null</c> if the user is not found.
    /// </returns>
    public Task<RestUser> GetUserAsync(ulong id, RequestOptions? options = null) =>
        ClientHelper.GetUserAsync(this, id, options);

    /// <summary>
    ///     Gets a user from a guild.
    /// </summary>
    /// <param name="guildId"> The identifier of the guild where the user is located. </param>
    /// <param name="id"> The identifier of the user (e.g. `168693960628371456`). </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the user from a guild
    ///     associated with the identifier; <c>null</c> if the user is not found in the guild.
    /// </returns>
    public Task<RestGuildUser?> GetGuildUserAsync(ulong guildId, ulong id, RequestOptions? options = null) =>
        ClientHelper.GetGuildMemberAsync(this, guildId, id, options);

    #endregion

    #region Friends

    /// <summary>
    ///     Gets friends.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a collection of users
    ///     that are friends with the current user.
    /// </returns>
    public Task<IReadOnlyCollection<RestUser>> GetFriendsAsync(RequestOptions? options = null) =>
        ClientHelper.GetFriendsAsync(this, options);

    /// <summary>
    ///     Gets friend requests.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a collection of
    ///     friend requests that the current user has received.
    /// </returns>
    public Task<IReadOnlyCollection<RestFriendRequest>> GetFriendRequestsAsync(RequestOptions? options = null) =>
        ClientHelper.GetFriendRequestsAsync(this, options);

    /// <summary>
    ///     Gets blocked users.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a collection of users
    ///     that are blocked by the current user.
    /// </returns>
    public Task<IReadOnlyCollection<RestUser>> GetBlockedUsersAsync(RequestOptions? options = null) =>
        ClientHelper.GetBlockedUsersAsync(this, options);

    #endregion

    #region Reactions

    /// <summary>
    ///     Adds a reaction to a message.
    /// </summary>
    /// <param name="messageId"> The identifier of the message. </param>
    /// <param name="emote"> The emoji used to react to the message. </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous operation for adding a reaction to the message.
    /// </returns>
    /// <seealso cref="IEmote"/>
    public Task AddReactionAsync(Guid messageId, IEmote emote, RequestOptions? options = null) =>
        MessageHelper.AddReactionAsync(messageId, emote, this, options);

    /// <summary>
    ///     Removes a reaction from a message.
    /// </summary>
    /// <param name="messageId"> The identifier of the message. </param>
    /// <param name="userId"> The identifier of the user who added the reaction. </param>
    /// <param name="emote"> The emoji used to remove from the message. </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous operation for removing a reaction from the message.
    /// </returns>
    /// <seealso cref="IEmote"/>
    public Task RemoveReactionAsync(Guid messageId, ulong userId, IEmote emote, RequestOptions? options = null) =>
        MessageHelper.RemoveReactionAsync(messageId, userId, emote, this, options);

    /// <summary>
    ///     Adds a reaction to a direct message.
    /// </summary>
    /// <param name="messageId"> The identifier of the direct message. </param>
    /// <param name="emote"> The emoji used to react to the message. </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous operation for adding a reaction to the direct message.
    /// </returns>
    /// <seealso cref="IEmote"/>
    public Task AddDirectMessageReactionAsync(Guid messageId, IEmote emote, RequestOptions? options = null) =>
        MessageHelper.AddDirectMessageReactionAsync(messageId, emote, this, options);

    /// <summary>
    ///     Removes a reaction from a direct message.
    /// </summary>
    /// <param name="messageId"> The identifier of the direct message. </param>
    /// <param name="userId"> The identifier of the user who added the reaction. </param>
    /// <param name="emote"> The emoji used to remove from the message. </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous operation for removing a reaction from the direct message.
    /// </returns>
    /// <seealso cref="IEmote"/>
    public Task RemoveDirectMessageReactionAsync(Guid messageId, ulong userId, IEmote emote, RequestOptions? options = null) =>
        MessageHelper.RemoveDirectMessageReactionAsync(messageId, userId, emote, this, options);

    #endregion

    #region Assets

    /// <summary>
    ///     Creates an asset from a file path.
    /// </summary>
    /// <param name="path"> The path to the file. </param>
    /// <param name="filename"> The name of the file. </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> The asset resource URI of the uploaded file. </returns>
    public async Task<string> CreateAssetAsync(string path, string? filename = null, RequestOptions? options = null)
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        await using FileStream fileStream = File.OpenRead(path);
#else
        using FileStream fileStream = File.OpenRead(path);
#endif
        return await ClientHelper.CreateAssetAsync(this, fileStream, filename ?? Path.GetFileName(path), options)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Creates an asset from a stream.
    /// </summary>
    /// <param name="stream"> The stream to the file. </param>
    /// <param name="filename"> The name of the file. </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> The asset resource URI of the uploaded file. </returns>
    public Task<string> CreateAssetAsync(Stream stream, string filename, RequestOptions? options = null) =>
        ClientHelper.CreateAssetAsync(this, stream, filename, options);

    #endregion

    #region Games

    /// <summary>
    ///     Gets games information.
    /// </summary>
    /// <param name="source">
    ///     Specifies whether to return games information created by the current user or
    ///     by the system by default; <c>null</c> to return all games information.
    /// </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A collection of games information. </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGame>> GetGamesAsync(
        GameCreationSource? source = null, RequestOptions? options = null) =>
        ClientHelper.GetGamesAsync(this, source, options);

    /// <summary>
    ///     Creates game information.
    /// </summary>
    /// <param name="name"> The name of the game. </param>
    /// <param name="processName"> The process name of the game. </param>
    /// <param name="iconUrl"> The icon URI of the game. </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns></returns>
    public Task<RestGame> CreateGameAsync(string name,
        string? processName = null, string? iconUrl = null, RequestOptions? options = null) =>
        ClientHelper.CreateGameAsync(this, name, processName, iconUrl, options);

    #endregion

    #region IKookClient

    /// <inheritdoc />
    async Task<IGuild?> IKookClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetGuildAsync(id, options).ConfigureAwait(false);
        return null;
    }

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IGuild>> IKookClient.GetGuildsAsync(CacheMode mode, RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetGuildsAsync(options).ConfigureAwait(false);
        return ImmutableArray.Create<IGuild>();
    }

    /// <inheritdoc />
    async Task<IUser?> IKookClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetUserAsync(id, options).ConfigureAwait(false);
        return null;
    }

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IUser>> IKookClient.GetFriendsAsync(CacheMode mode, RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetFriendsAsync(options).ConfigureAwait(false);
        return [];
    }

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IFriendRequest>> IKookClient.GetFriendRequestsAsync(CacheMode mode,
        RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetFriendRequestsAsync(options).ConfigureAwait(false);
        return [];
    }

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IUser>> IKookClient.GetBlockedUsersAsync(CacheMode mode,
        RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetBlockedUsersAsync(options).ConfigureAwait(false);
        return [];
    }

    /// <inheritdoc />
    async Task<IChannel?> IKookClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetChannelAsync(id, options).ConfigureAwait(false);
        return null;
    }

    /// <inheritdoc />
    async Task<IDMChannel?> IKookClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetDMChannelAsync(chatCode, options).ConfigureAwait(false);
        return null;
    }

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IDMChannel>> IKookClient.GetDMChannelsAsync(CacheMode mode,
        RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetDMChannelsAsync(options).ConfigureAwait(false);
        return ImmutableArray.Create<IDMChannel>();
    }

    #endregion
}
