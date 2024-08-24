using Kook.API;
using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的私聊频道。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketDMChannel : SocketChannel, IDMChannel, ISocketPrivateChannel, ISocketMessageChannel
{
    #region SocketDMChannel

    /// <summary>
    ///     获取此私聊频道的唯一标识符。
    /// </summary>
    /// <remarks>
    ///     此属性的值与 <see cref="P:Kook.WebSocket.SocketDMChannel.ChatCode"/> 相同。
    /// </remarks>
    public new Guid Id { get; }

    /// <summary>
    ///     获取此私聊频道的聊天代码。
    /// </summary>
    /// <remarks>
    ///     此属性的值与 <see cref="P:Kook.WebSocket.SocketDMChannel.Id"/> 相同。
    /// </remarks>
    public Guid ChatCode => Id;

    /// <inheritdoc cref="P:Kook.IDMChannel.Recipient" />
    public SocketUser Recipient { get; }

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="important">
    ///         私聊消息频道不支持缓存消息，此属性将始终返回空集合。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<SocketMessage> CachedMessages => ImmutableArray.Create<SocketMessage>();

    /// <inheritdoc cref="P:Kook.WebSocket.SocketChannel.Users" />
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
    /// <remarks>
    ///     <note type="important">
    ///         私聊消息频道不支持缓存消息，此方法将始终返回 <c>null</c>。
    ///     </note>
    /// </remarks>
    public SocketMessage? GetCachedMessage(Guid id) => null;

    /// <summary>
    ///     从此消息频道获取一条消息。
    /// </summary>
    /// <param name="id"> 消息的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务结果包含检索到的消息；如果未找到具有指定 ID 的消息，则返回 <c>null</c>。 </returns>
    public async Task<IMessage> GetMessageAsync(Guid id, RequestOptions? options = null) =>
        await ChannelHelper.GetDirectMessageAsync(this, Kook, id, options).ConfigureAwait(false);

    /// <summary>
    ///     获取此消息频道中的最新的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, null, Direction.Before, limit, true, options);

    /// <summary>
    ///     获取此消息频道中的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceMessageId"> 要开始获取消息的参考位置的消息的 ID。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, referenceMessageId, dir, limit, true, options);

    /// <summary>
    ///     获取此消息频道中的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceMessage"> 要开始获取消息的参考位置的消息。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, referenceMessage.Id, dir, limit, true, options);

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="important">
    ///         私聊消息频道不支持缓存消息，此属性将始终返回空集合。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = KookConfig.MaxMessagesPerBatch) => [];

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="important">
    ///         私聊消息频道不支持缓存消息，此属性将始终返回空集合。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(Guid referenceMessageId,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch) => [];

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="important">
    ///         私聊消息频道不支持缓存消息，此属性将始终返回空集合。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage referenceMessage,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch) => [];

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendFileAsync(System.String,System.String,Kook.AttachmentType,Kook.IQuote,Kook.IUser,Kook.RequestOptions)" />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, path, filename, type, quote, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendFileAsync(System.IO.Stream,System.String,Kook.AttachmentType,Kook.IQuote,Kook.IUser,Kook.RequestOptions)" />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, stream, filename, type, quote, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendFileAsync(Kook.FileAttachment,Kook.IQuote,Kook.IUser,Kook.RequestOptions)" />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, attachment, quote, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendTextAsync(System.String,Kook.IQuote,Kook.IUser,Kook.RequestOptions)" />
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.KMarkdown, text, quote, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendCardsAsync(System.Collections.Generic.IEnumerable{Kook.ICard},Kook.IQuote,Kook.IUser,Kook.RequestOptions)" />
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectCardsAsync(this, Kook, cards, quote, options);

    /// <inheritdoc cref="M:Kook.IMessageChannel.SendCardAsync(Kook.ICard,Kook.IQuote,Kook.IUser,Kook.RequestOptions)" />
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

    /// <inheritdoc cref="M:Kook.WebSocket.SocketChannel.GetUser(System.UInt64)" />
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
    ///     获取此参与到此私聊频道的另外一位用户的包含 <c>@</c> 前缀的用户名及识别号格式化字符串。
    /// </summary>
    /// <returns> 一个表示此私聊频道的格式化字符串。 </returns>
    /// <seealso cref="M:Kook.Format.UsernameAndIdentifyNumber(Kook.IUser,System.Boolean)"/>
    public override string ToString() => $"@{Recipient}";

    private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
    internal new SocketDMChannel Clone() => (SocketDMChannel)MemberwiseClone();
}
