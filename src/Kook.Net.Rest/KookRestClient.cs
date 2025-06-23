using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的 KOOK 客户端。
/// </summary>
public class KookRestClient : BaseKookClient, IKookClient
{
    #region KookRestClient

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <inheritdoc cref="Kook.Rest.BaseKookClient.CurrentUser" />
    public new RestSelfUser? CurrentUser
    {
        get => base.CurrentUser as RestSelfUser;
        internal set => base.CurrentUser = value;
    }

    /// <summary>
    ///     使用默认配置初始化一个 <see cref="KookRestClient"/> 类的新实例。
    /// </summary>
    public KookRestClient()
        : this(new KookRestConfig())
    {
    }

    /// <summary>
    ///     使用指定的配置初始化一个 <see cref="KookRestClient"/> 类的新实例。
    /// </summary>
    /// <param name="config"> 用于初始化客户端的配置。 </param>
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
    ///     获取一个服务器。
    /// </summary>
    /// <param name="id"> 服务器的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定 ID 的服务器；若指定 ID 的服务器不存在，则为 <c>null</c>。 </returns>
    public Task<RestGuild> GetGuildAsync(ulong id, RequestOptions? options = null) =>
        ClientHelper.GetGuildAsync(this, id, options);

    /// <summary>
    ///     获取当前用户所在的所有服务器。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是当前用户所在的所有服务器。 </returns>
    public Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(RequestOptions? options = null) =>
        ClientHelper.GetGuildsAsync(this, options);

    #endregion

    #region Channels

    /// <summary>
    ///     获取一个频道。
    /// </summary>
    /// <param name="id"> 频道的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定 ID 的频道；若指定 ID 的频道不存在，则为 <c>null</c>。 </returns>
    public Task<RestChannel> GetChannelAsync(ulong id, RequestOptions? options = null) =>
        ClientHelper.GetChannelAsync(this, id, options);

    /// <summary>
    ///     获取一个私聊频道。
    /// </summary>
    /// <param name="chatCode"> 私聊频道的聊天代码。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定聊天代码的私聊频道；若指定聊天代码的私聊频道不存在，则为 <c>null</c>。 </returns>
    public Task<RestDMChannel> GetDMChannelAsync(Guid chatCode, RequestOptions? options = null) =>
        ClientHelper.GetDMChannelAsync(this, chatCode, options);

    /// <summary>
    ///     获取当前会话中已创建的所有私聊频道。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         此方法不会返回当前会话之外已创建的私聊频道。如果客户端刚刚启动，这可能会返回一个空集合。
    ///     </note>
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是当前会话中已创建的所有私聊频道。 </returns>
    public Task<IReadOnlyCollection<RestDMChannel>> GetDMChannelsAsync(RequestOptions? options = null) =>
        ClientHelper.GetDMChannelsAsync(this, options);

    #endregion

    #region Roles

    /// <summary>
    ///     在指定服务器内授予指定用户指定的角色。
    /// </summary>
    /// <param name="guildId"> 要授予的角色及服务器用户所在的服务器的 ID。 </param>
    /// <param name="userId"> 要为其授予角色的用户的 ID。 </param>
    /// <param name="roleId"> 要授予的角色的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步授予操作的任务。 </returns>
    public Task AddRoleAsync(ulong guildId, ulong userId, uint roleId, RequestOptions? options = null) =>
        ClientHelper.AddRoleAsync(this, guildId, userId, roleId);

    /// <summary>
    ///     在指定服务器内撤销指定用户指定的角色。
    /// </summary>
    /// <param name="guildId"> 要撤销的角色及服务器用户所在的服务器的 ID。 </param>
    /// <param name="userId"> 要为其撤销角色的用户的 ID。 </param>
    /// <param name="roleId"> 要撤销的角色的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步撤销操作的任务。 </returns>
    public Task RemoveRoleAsync(ulong guildId, ulong userId, uint roleId, RequestOptions? options = null) =>
        ClientHelper.RemoveRoleAsync(this, guildId, userId, roleId);

    #endregion

    #region Users

    /// <summary>
    ///     获取一个用户。
    /// </summary>
    /// <param name="id"> 用户的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定 ID 的用户；若指定 ID 的用户不存在，则为 <c>null</c>。 </returns>
    public Task<RestUser> GetUserAsync(ulong id, RequestOptions? options = null) =>
        ClientHelper.GetUserAsync(this, id, options);

    /// <summary>
    ///     获取一个服务器用户。
    /// </summary>
    /// <param name="guildId"> 服务器的 ID。 </param>
    /// <param name="id"> 用户的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是在具有指定 ID 的服务器内具有指定 ID 的用户；若指定 ID 的服务器内指定 ID 的用户不存在，则为 <c>null</c>。 </returns>
    public Task<RestGuildUser?> GetGuildUserAsync(ulong guildId, ulong id, RequestOptions? options = null) =>
        ClientHelper.GetGuildMemberAsync(this, guildId, id, options);

    #endregion

    #region Friends

    /// <summary>
    ///     获取所有好友。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有与当前用户是好友的用户。 </returns>
    public Task<IReadOnlyCollection<RestUser>> GetFriendsAsync(RequestOptions? options = null) =>
        ClientHelper.GetFriendsAsync(this, options);

    /// <summary>
    ///     获取所有好友请求。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有请求与当前用户成为好友的用户。 </returns>
    public Task<IReadOnlyCollection<RestFriendRequest>> GetFriendRequestsAsync(RequestOptions? options = null) =>
        ClientHelper.GetFriendRequestsAsync(this, options);

    /// <summary>
    ///     获取所有被当前用户屏蔽的用户。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有被当前用户屏蔽的用户。 </returns>
    public Task<IReadOnlyCollection<RestUser>> GetBlockedUsersAsync(RequestOptions? options = null) =>
        ClientHelper.GetBlockedUsersAsync(this, options);

    /// <summary>
    ///     获取所有与当前用于建立了的亲密关系。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有与当前用户建立了的亲密关系。 </returns>
    public Task<IReadOnlyCollection<RestIntimacyRelation>> GetIntimacyRelationsAsync(RequestOptions? options = null) =>
        ClientHelper.GetIntimacyUsersAsync(this, options);

    /// <summary>
    ///     获取所有请求与当前用户建立亲密关系的用户。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有请求与当前用户建立亲密关系的用户。 </returns>
    public Task<IReadOnlyCollection<RestIntimacyRelationRequest>> GetIntimacyRelationRequestsAsync(RequestOptions? options = null) =>
        ClientHelper.GetIntimacyRequestsAsync(this, options);

    #endregion

    #region Message Templates

    /// <summary>
    ///     获取所有消息模板。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是所有消息模板的只读集合。 </returns>
    public Task<IReadOnlyCollection<RestMessageTemplate>> GetMessageTemplatesAsync(RequestOptions? options = null) =>
        MessageTemplateHelper.GetMessageTemplatesAsync(this, options);

    /// <summary>
    ///     获取指定的消息模板。
    /// </summary>
    /// <param name="id"> 消息模板的 ID。</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果是具有指定 ID 的消息模板。 </returns>
    public Task<RestMessageTemplate> GetMessageTemplateAsync(ulong id, RequestOptions? options = null) =>
        MessageTemplateHelper.GetMessageTemplateAsync(this, id, options);

    /// <summary>
    ///     创建一个新的消息模板。
    /// </summary>
    /// <param name="title"> 消息模板的标题。</param>
    /// <param name="content"> 消息模板的内容。</param>
    /// <param name="type"> 消息模板的类型。</param>
    /// <param name="messageType"> 消息模板的消息类型。</param>
    /// <param name="testChannelId"> 测试频道 ID。</param>
    /// <param name="testData"> 测试数据。</param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果是所创建的消息模板。 </returns>
    public Task<RestMessageTemplate> CreateMessageTemplateAsync(string title, string content,
        TemplateType type = TemplateType.Twig, TemplateMessageType messageType = TemplateMessageType.KMarkdown,
        ulong? testChannelId = null, JsonElement? testData = null) =>
        MessageTemplateHelper.CreateAsync(this, title, content, type, messageType, testChannelId, testData);

    #endregion

    #region Reactions

    /// <summary>
    ///     向指定的消息添加一个回应。
    /// </summary>
    /// <param name="messageId"> 要为其添加回应的消息的 ID。 </param>
    /// <param name="emote"> 要用于向指定消息添加回应的表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示添加添加异步操作的任务。 </returns>
    public Task AddReactionAsync(Guid messageId, IEmote emote, RequestOptions? options = null) =>
        MessageHelper.AddReactionAsync(messageId, emote, this, options);

    /// <summary>
    ///     从指定的消息移除一个回应。
    /// </summary>
    /// <param name="messageId"> 要从中移除回应的消息的 ID。 </param>
    /// <param name="userId"> 要移除其回应的用户的 ID。 </param>
    /// <param name="emote"> 要从指定消息移除的回应的表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除操作的任务。 </returns>
    public Task RemoveReactionAsync(Guid messageId, ulong userId, IEmote emote, RequestOptions? options = null) =>
        MessageHelper.RemoveReactionAsync(messageId, userId, emote, this, options);

    /// <summary>
    ///     向指定的私聊消息添加一个回应。
    /// </summary>
    /// <param name="messageId"> 要为其添加回应的消息的 ID。 </param>
    /// <param name="emote"> 要用于向指定消息添加回应的表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示添加添加异步操作的任务。 </returns>
    public Task AddDirectMessageReactionAsync(Guid messageId, IEmote emote, RequestOptions? options = null) =>
        MessageHelper.AddDirectMessageReactionAsync(messageId, emote, this, options);

    /// <summary>
    ///     从指定的私聊消息移除一个回应。
    /// </summary>
    /// <param name="messageId"> 要从中移除回应的消息的 ID。 </param>
    /// <param name="userId"> 要移除其回应的用户的 ID。 </param>
    /// <param name="emote"> 要从指定消息移除的回应的表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除操作的任务。 </returns>
    public Task RemoveDirectMessageReactionAsync(Guid messageId, ulong userId, IEmote emote, RequestOptions? options = null) =>
        MessageHelper.RemoveDirectMessageReactionAsync(messageId, userId, emote, this, options);

    #endregion

    #region Assets

    /// <summary>
    ///     从文件路径上传并创建一个资源。
    /// </summary>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果是上传文件后的资源地址 URL。 </returns>
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
    ///     从文件的流上传并创建一个资源。
    /// </summary>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果是上传文件后的资源地址 URL。 </returns>
    public Task<string> CreateAssetAsync(Stream stream, string filename, RequestOptions? options = null) =>
        ClientHelper.CreateAssetAsync(this, stream, filename, options);

    #endregion

    #region Games

    /// <summary>
    ///     获取所有游戏信息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取所有具有指定创建源的游戏信息。此方法会根据 <see cref="Kook.KookConfig.MaxItemsPerBatchByDefault"/>
    ///     将请求拆分。换句话说，如果有 3000 款游戏的信息，而 <see cref="Kook.KookConfig.MaxItemsPerBatchByDefault"/> 的常量为
    ///     <c>100</c>，则请求将被拆分为 30 个单独请求，因此异步枚举器会异步枚举返回 30 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 30 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="source"> 要获取的游戏信息的创建来源。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的游戏信息集合的异步可枚举对象。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGame>> GetGamesAsync(
        GameCreationSource? source = null, RequestOptions? options = null) =>
        ClientHelper.GetGamesAsync(this, source, options);

    /// <summary>
    ///     创建一款游戏的信息。
    /// </summary>
    /// <param name="name"> 游戏的名称。 </param>
    /// <param name="processName"> 游戏进程的名称。 </param>
    /// <param name="iconUrl"> 游戏图标的资源地址 URL。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步创建操作的任务。任务的结果是所创建的游戏信息。 </returns>
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
    async Task<IReadOnlyCollection<IIntimacyRelation>> IKookClient.GetIntimacyRelationsAsync(CacheMode mode, RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetIntimacyRelationsAsync(options).ConfigureAwait(false);
        return [];
    }

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IFriendRequest>> IKookClient.GetIntimacyRelationRequestsAsync(CacheMode mode,
        RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetIntimacyRelationRequestsAsync(options).ConfigureAwait(false);
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
