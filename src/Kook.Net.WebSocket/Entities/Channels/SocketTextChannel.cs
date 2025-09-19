using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     表示服务器中一个基于网关的具有文字聊天能力的频道，可以发送和接收消息。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketTextChannel : SocketGuildChannel, ITextChannel, ISocketMessageChannel
{
    #region SocketTextChannel

    private readonly MessageCache? _messages;

    /// <inheritdoc />
    public string Topic { get; private set; }

    /// <inheritdoc />
    public int SlowModeInterval { get; private set; }

    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }

    /// <summary>
    ///     获取此嵌套频道在服务器频道列表中所属的分组频道的。
    /// </summary>
    /// <remarks> 如果当前频道不属于任何分组频道，则会返回 <c>null</c>。 </remarks>
    public ICategoryChannel? Category => CategoryId.HasValue
        ? Guild.GetChannel(CategoryId.Value) as ICategoryChannel
        : null;

    /// <inheritdoc />
    public bool? IsPermissionSynced { get; private set; }

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> CachedMessages => _messages?.Messages ?? [];

    /// <inheritdoc />
    public override IReadOnlyCollection<SocketGuildUser> Users => Guild.Users
        .Where(x => Permissions.GetValue(
            Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
            ChannelPermission.ViewChannel))
        .ToImmutableArray();

    internal SocketTextChannel(KookSocketClient kook, ulong id, SocketGuild guild)
        : base(kook, id, guild)
    {
        Type = ChannelType.Text;
        Topic = string.Empty;
        if (Kook.MessageCacheSize > 0)
            _messages = new MessageCache(Kook);
    }

    internal static new SocketTextChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        SocketTextChannel entity = new(guild.Kook, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }

    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        Topic = model.Topic ?? string.Empty;
        SlowModeInterval = model.SlowMode / 1000;
        IsPermissionSynced = model.PermissionSync;
    }

    /// <inheritdoc />
    public virtual Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions? options = null) =>
        ChannelHelper.ModifyAsync(this, Kook, func, options);

    /// <inheritdoc />
    public virtual Task SyncPermissionsAsync(RequestOptions? options = null) =>
        ChannelHelper.SyncPermissionsAsync(this, Kook, options);

    private string DebuggerDisplay => $"{Name} ({Id}, Text)";
    internal new SocketTextChannel Clone() => (SocketTextChannel)MemberwiseClone();

    #endregion

    #region Messages

    /// <inheritdoc />
    public SocketMessage? GetCachedMessage(Guid id) => _messages?.Get(id);

    /// <summary>
    ///     从此消息频道获取一条消息。
    /// </summary>
    /// <param name="id"> 消息的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务结果包含检索到的消息；如果未找到具有指定 ID 的消息，则返回 <c>null</c>。 </returns>
    public async Task<IMessage> GetMessageAsync(Guid id, RequestOptions? options = null)
    {
        IMessage? msg = _messages?.Get(id);
        return msg ?? await ChannelHelper.GetMessageAsync(this, Kook, id, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     获取此消息频道中的最新的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        SocketChannelHelper.GetMessagesAsync(this, Kook, _messages,
            null, Direction.Before, limit, CacheMode.AllowDownload, options);

    /// <summary>
    ///     获取此消息频道中的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceMessageId"> 要开始获取消息的参考位置的消息的 ID。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        SocketChannelHelper.GetMessagesAsync(this, Kook, _messages,
            referenceMessageId, dir, limit, CacheMode.AllowDownload, options);

    /// <summary>
    ///     获取此消息频道中的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceMessage"> 要开始获取消息的参考位置的消息。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        SocketChannelHelper.GetMessagesAsync(this, Kook, _messages,
            referenceMessage.Id, dir, limit, CacheMode.AllowDownload, options);

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = KookConfig.MaxMessagesPerBatch) =>
        SocketChannelHelper.GetCachedMessages(this, Kook, _messages, null, Direction.Before, limit);

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(Guid referenceMessageId,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch) =>
        SocketChannelHelper.GetCachedMessages(this, Kook, _messages, referenceMessageId, dir, limit);

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage referenceMessage,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch) =>
        SocketChannelHelper.GetCachedMessages(this, Kook, _messages, referenceMessage.Id, dir, limit);

    /// <inheritdoc cref="ITextChannel.GetPinnedMessagesAsync(RequestOptions)"/>
    public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions? options = null) =>
        ChannelHelper.GetPinnedMessagesAsync(this, Kook, options);

    /// <inheritdoc cref="Kook.IMessageChannel.SendFileAsync(System.String,System.String,Kook.AttachmentType,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, IUser? ephemeralUser = null,
        RequestOptions? options = null)
    {
        string name = filename ?? Path.GetFileName(path);
        return ChannelHelper.SendFileAsync(this, Kook, path, name, type, quote, ephemeralUser, options);
    }

    /// <inheritdoc cref="Kook.IMessageChannel.SendFileAsync(System.IO.Stream,System.String,Kook.AttachmentType,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, IUser? ephemeralUser = null,
        RequestOptions? options = null) =>
        ChannelHelper.SendFileAsync(this, Kook, stream, filename, type, quote, ephemeralUser, options);

    /// <inheritdoc cref="Kook.IMessageChannel.SendFileAsync(Kook.FileAttachment,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        ChannelHelper.SendFileAsync(this, Kook, attachment, quote, ephemeralUser, options);

    /// <inheritdoc cref="Kook.IMessageChannel.SendTextAsync(System.String,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        ChannelHelper.SendMessageAsync(this, Kook, MessageType.KMarkdown, text, quote, ephemeralUser, options);

    /// <inheritdoc cref="Kook.IMessageChannel.SendTextAsync{T}(System.UInt64,T,Kook.IQuote,Kook.IUser,System.Text.Json.JsonSerializerOptions,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync<T>(ulong templateId, T parameters, IQuote? quote = null,
        IUser? ephemeralUser = null, JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        ChannelHelper.SendMessageAsync(this, Kook, MessageType.KMarkdown, templateId, parameters, quote, ephemeralUser, jsonSerializerOptions, options);

    /// <inheritdoc cref="Kook.IMessageChannel.SendCardsAsync(System.Collections.Generic.IEnumerable{Kook.ICard},Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        ChannelHelper.SendCardsAsync(this, Kook, cards, quote, ephemeralUser, options);

    /// <inheritdoc cref="Kook.IMessageChannel.SendCardsAsync{T}(System.UInt64,T,Kook.IQuote,Kook.IUser,System.Text.Json.JsonSerializerOptions,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync<T>(ulong templateId, T parameters, IQuote? quote = null,
        IUser? ephemeralUser = null, JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        ChannelHelper.SendCardsAsync(this, Kook, templateId, parameters, quote, ephemeralUser, jsonSerializerOptions, options);

    /// <inheritdoc cref="Kook.IMessageChannel.SendCardAsync(Kook.ICard,Kook.IQuote,Kook.IUser,Kook.RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null) =>
        ChannelHelper.SendCardAsync(this, Kook, card, quote, ephemeralUser, options);

    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId,
        Action<MessageProperties> func, RequestOptions? options = null) =>
        await ChannelHelper.ModifyMessageAsync<object>(this, messageId, func, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task ModifyMessageAsync<T>(Guid messageId,
        Action<MessageProperties<T>> func, RequestOptions? options = null) =>
        await ChannelHelper.ModifyMessageAsync(this, messageId, func, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task PinMessageAsync(IUserMessage message, RequestOptions? options = null) =>
        await MessageHelper.PinAsync(message.Id, Id, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task PinMessageAsync(Guid messageId, RequestOptions? options = null) =>
        await MessageHelper.PinAsync(messageId, Id, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task UnpinMessageAsync(IUserMessage message, RequestOptions? options = null) =>
        await MessageHelper.UnpinAsync(message.Id, Id, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task UnpinMessageAsync(Guid messageId, RequestOptions? options = null) =>
        await MessageHelper.UnpinAsync(messageId, Id, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions? options = null) =>
        ChannelHelper.DeleteMessageAsync(this, messageId, Kook, options);

    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null) =>
        ChannelHelper.DeleteMessageAsync(this, message.Id, Kook, options);

    internal void AddMessage(SocketMessage msg) => _messages?.Add(msg);

    internal SocketMessage? RemoveMessage(Guid id) => _messages?.Remove(id);

    #endregion

    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions? options = null) =>
        await ChannelHelper.GetInvitesAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800,
        int? maxUses = null, RequestOptions? options = null) =>
        await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800,
        InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions? options = null) =>
        await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    #region Users

    /// <inheritdoc />
    public override SocketGuildUser? GetUser(ulong id)
    {
        if (Guild.GetUser(id) is not { } user) return null;
        ulong guildPerms = Permissions.ResolveGuild(Guild, user);
        ulong channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
        return Permissions.GetValue(channelPerms, ChannelPermission.ViewChannel) ? user : null;
    }

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    async Task<IGuildUser?> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options)
    {
        if (GetUser(id) is { } user)
            return user;
        if (mode == CacheMode.CacheOnly)
            return null;
        return await ChannelHelper.GetUserAsync(this, Guild, Kook, id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(
        CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();

    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(string path, string? filename,
        AttachmentType type, IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(path, filename, type, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(Stream stream, string filename,
        AttachmentType type, IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(stream, filename, type, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(FileAttachment attachment,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(attachment, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendTextAsync(string text,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendTextAsync(text, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendTextAsync<T>(ulong templateId, T parameters,
        IQuote? quote, IUser? ephemeralUser, JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options) =>
        SendTextAsync(templateId, parameters, quote, ephemeralUser, jsonSerializerOptions, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardAsync(ICard card,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendCardAsync(card, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendCardsAsync(cards, quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardsAsync<T>(ulong templateId, T parameters,
        IQuote? quote, IUser? ephemeralUser, JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options) =>
        SendCardsAsync(templateId, parameters, quote, ephemeralUser, jsonSerializerOptions, options);

    /// <inheritdoc />
    async Task<IMessage?> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetMessageAsync(id, options).ConfigureAwait(false)
            : GetCachedMessage(id);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(
        int limit, CacheMode mode, RequestOptions? options) =>
        SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, null, Direction.Before, limit, mode, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId,
        Direction dir, int limit, CacheMode mode, RequestOptions? options) =>
        SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, referenceMessageId, dir, limit, mode, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage,
        Direction dir, int limit, CacheMode mode, RequestOptions? options) =>
        SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, referenceMessage.Id, dir, limit, mode, options);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IMessage>> ITextChannel.GetPinnedMessagesAsync(RequestOptions? options) =>
        await GetPinnedMessagesAsync(options).ConfigureAwait(false);

    #endregion

    #region INestedChannel

    /// <inheritdoc />
    Task<ICategoryChannel?> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult(Category);

    #endregion
}
