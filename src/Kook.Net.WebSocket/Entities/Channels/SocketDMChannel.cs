using Kook.API;
using Kook.Rest;
using System.Collections.Immutable;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based direct-message channel.
/// </summary>
public class SocketDMChannel : SocketChannel, IDMChannel, ISocketPrivateChannel, ISocketMessageChannel
{
    #region SocketDMChannel

    /// <summary>
    ///     Get the identifier of the DM channel.
    /// </summary>
    /// <remarks>
    ///     This property is the same as <see cref="ChatCode" />.
    /// </remarks>
    public new Guid Id { get; }

    /// <inheritdoc />
    /// <remarks>
    ///     This property is the same as <see cref="Id" />.
    /// </remarks>
    public Guid ChatCode => Id;

    /// <summary>
    ///     Gets the recipient of the channel.
    /// </summary>
    public SocketUser Recipient { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> CachedMessages => ImmutableArray.Create<SocketMessage>();

    /// <summary>
    ///     Gets a collection that is the current logged-in user and the recipient.
    /// </summary>
    public new IReadOnlyCollection<SocketUser> Users => ImmutableArray.Create(Kook.CurrentUser, Recipient);

    internal SocketDMChannel(KookSocketClient kook, Guid chatCode, SocketUser recipient)
        : base(kook, default)
    {
        Id = chatCode;
        Recipient = recipient;
    }

    internal static SocketDMChannel Create(KookSocketClient kook, ClientState state, Guid chatCode, User recipient)
    {
        SocketDMChannel entity = new(kook, chatCode, kook.GetOrCreateTemporaryUser(state, recipient));
        entity.Update(state, recipient);
        return entity;
    }

    internal void Update(ClientState state, User recipient)
    {
        Recipient.Update(state, recipient);
        Recipient.UpdatePresence(recipient.Online, recipient.OperatingSystem);
    }

    internal void Update(ClientState state, UserChat model) => Recipient.Update(state, model.Recipient);

    /// <inheritdoc />
    public override Task UpdateAsync(RequestOptions? options = null) =>
        SocketChannelHelper.UpdateAsync(this, options);

    /// <inheritdoc />
    public Task CloseAsync(RequestOptions? options = null) =>
        ChannelHelper.DeleteDMChannelAsync(this, Kook, options);

    #endregion

    #region Messages

    /// <inheritdoc />
    public SocketMessage? GetCachedMessage(Guid id) => null;

    /// <summary>
    ///     Gets the message associated with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">TThe ID of the message.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     The message gotten from either the cache or the download, or <c>null</c> if none is found.
    /// </returns>
    public async Task<IMessage> GetMessageAsync(Guid id, RequestOptions? options = null) =>
        await ChannelHelper.GetDirectMessageAsync(this, Kook, id, options).ConfigureAwait(false);

    /// <summary>
    ///     Gets the last N messages from this message channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(int, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     分页的消息集合的异步可枚举对象。
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, null, Direction.Before, limit, true, options);

    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(Guid, Direction, int, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="referenceMessageId"> 要开始获取消息的参考位置的消息的 ID。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     分页的消息集合的异步可枚举对象。
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, referenceMessageId, dir, limit, true, options);

    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(IMessage, Direction, int, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="referenceMessage"> 要开始获取消息的参考位置的消息。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     分页的消息集合的异步可枚举对象。
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, referenceMessage.Id, dir, limit, true, options);

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = KookConfig.MaxMessagesPerBatch) => [];

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(Guid fromMessageId,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch) => [];

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch) => [];

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, path, filename, type, quote, options);

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, stream, filename, type, quote, options);

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, attachment, quote, options);

    /// <summary>
    ///     发送文本消息到此消息频道。
    /// </summary>
    /// <param name="text">The message to be sent.</param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.KMarkdown, text, quote, options);

    /// <summary>
    ///     发送卡片消息到此消息频道。
    /// </summary>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectCardsAsync(this, Kook, cards, quote, options);

    /// <summary>
    ///     发送卡片消息到此消息频道。
    /// </summary>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectCardAsync(this, Kook, card, quote, options);

    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId,
        Action<MessageProperties> func, RequestOptions? options = null) =>
        await ChannelHelper.ModifyDirectMessageAsync(this, messageId, func, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions? options = null) =>
        ChannelHelper.DeleteDirectMessageAsync(this, messageId, Kook, options);

    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null) =>
        ChannelHelper.DeleteDirectMessageAsync(this, message.Id, Kook, options);

    internal void AddMessage(SocketMessage msg)
    {
    }

    internal SocketMessage? RemoveMessage(Guid id) => null;

    #endregion

    #region Users

    /// <summary>
    ///     Gets a user in this channel from the provided <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>
    ///     A <see cref="SocketUser"/> object that is a recipient of this channel; otherwise <c>null</c>.
    /// </returns>
    public new SocketUser? GetUser(ulong id)
    {
        if (id == Recipient.Id) return Recipient;
        return id == Kook.CurrentUser?.Id ? Kook.CurrentUser : null;
    }

    #endregion

    #region SocketChannel

    /// <inheritdoc />
    internal override void Update(ClientState state, Channel model) =>
        throw new NotSupportedException("Update a DMChannel via Channel is not supported");

    /// <inheritdoc />
    internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;

    /// <inheritdoc />
    internal override SocketUser? GetUserInternal(ulong id) => GetUser(id);

    #endregion

    #region IDMChannel

    /// <inheritdoc />
    IUser IDMChannel.Recipient => Recipient;

    #endregion

    #region ISocketPrivateChannel

    /// <inheritdoc />
    IReadOnlyCollection<SocketUser> ISocketPrivateChannel.Recipients => [Recipient];

    #endregion

    #region IPrivateChannel

    /// <inheritdoc />
    IReadOnlyCollection<IUser> IPrivateChannel.Recipients => [Recipient];

    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendFileAsync(string path, string? filename,
        AttachmentType type, IQuote? quote, RequestOptions? options) =>
        SendFileAsync(path, filename, type, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendFileAsync(Stream stream, string filename,
        AttachmentType type, IQuote? quote, RequestOptions? options) =>
        SendFileAsync(stream, filename, type, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendFileAsync(FileAttachment attachment,
        IQuote? quote, RequestOptions? options) =>
        SendFileAsync(attachment, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendTextAsync(string text,
        IQuote? quote, RequestOptions? options) =>
        SendTextAsync(text, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendCardAsync(ICard card,
        IQuote? quote, RequestOptions? options) =>
        SendCardAsync(card, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote, RequestOptions? options) =>
        SendCardsAsync(cards, quote, options);

    /// <inheritdoc />
    async Task<IMessage?> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetMessageAsync(id, options).ConfigureAwait(false)
            : GetCachedMessage(id);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit,
        CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.CacheOnly
            ? ImmutableArray<IReadOnlyCollection<IMessage>>.Empty.ToAsyncEnumerable()
            : GetMessagesAsync(limit, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId,
        Direction dir, int limit, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.CacheOnly
            ? ImmutableArray<IReadOnlyCollection<IMessage>>.Empty.ToAsyncEnumerable()
            : GetMessagesAsync(referenceMessageId, dir, limit, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage,
        Direction dir, int limit, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.CacheOnly
            ? ImmutableArray<IReadOnlyCollection<IMessage>>.Empty.ToAsyncEnumerable()
            : GetMessagesAsync(referenceMessage.Id, dir, limit, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(string path, string? filename,
        AttachmentType type, IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(path, filename, type, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(Stream stream, string filename,
        AttachmentType type, IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(stream, filename, type, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(FileAttachment attachment,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendFileAsync(attachment, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendTextAsync(string text,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendTextAsync(text, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendCardsAsync(cards, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardAsync(ICard card,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendCardAsync(card, quote, options);

    #endregion

    #region IChannel

    /// <inheritdoc />
    string IChannel.Name => $"@{Recipient}";

    /// <inheritdoc />
    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IUser?>(GetUser(id));

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();

    #endregion

    /// <summary>
    ///     Returns the recipient user.
    /// </summary>
    public override string ToString() => $"@{Recipient}";

    private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
    internal new SocketDMChannel Clone() => (SocketDMChannel)MemberwiseClone();
}
