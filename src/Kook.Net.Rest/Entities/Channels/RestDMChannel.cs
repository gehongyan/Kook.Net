using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.UserChat;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的私聊频道。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestDMChannel : RestChannel, IDMChannel, IRestPrivateChannel, IRestMessageChannel
{
    #region RestDMChannel

    /// <inheritdoc cref="Kook.IDMChannel.Id" />
    /// <remarks>
    ///     此属性的值与 <see cref="Kook.Rest.RestDMChannel.ChatCode"/> 相同。
    /// </remarks>
    public new Guid Id { get; }

    /// <inheritdoc />
    /// <remarks>
    ///     此属性的值与 <see cref="Kook.Rest.RestDMChannel.Id"/> 相同。
    /// </remarks>
    public Guid ChatCode => Id;

    /// <summary>
    ///     获取参与到此私聊频道中的当前用户。
    /// </summary>
    public RestUser CurrentUser { get; }

    /// <inheritdoc cref="Kook.IDMChannel.Recipient" />
    public RestUser Recipient { get; }

    /// <summary>
    ///     获取参与到此私聊频道中的所有用户。
    /// </summary>
    public IReadOnlyCollection<RestUser> Users => [CurrentUser, Recipient];

    internal RestDMChannel(BaseKookClient kook, Guid chatCode, ulong recipientId)
        : base(kook, default)
    {
        Id = chatCode;
        Recipient = new RestUser(Kook, recipientId);
        if (kook.CurrentUser is RestUser restUser)
            CurrentUser = restUser;
        else if (kook.CurrentUser is not null)
            CurrentUser = new RestUser(Kook, kook.CurrentUser.Id);
        else
            throw new InvalidOperationException("The current user is not set well via login.");
    }

    internal static RestDMChannel Create(BaseKookClient kook, Model model)
    {
        RestDMChannel entity = new(kook, model.Code, model.Recipient.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model) => Recipient.Update(model.Recipient);

    /// <inheritdoc />
    public override async Task UpdateAsync(RequestOptions? options = null)
    {
        Model model = await Kook.ApiClient.GetUserChatAsync(Id, options).ConfigureAwait(false);
        Update(model);
    }

    /// <inheritdoc />
    public Task CloseAsync(RequestOptions? options = null) =>
        ChannelHelper.DeleteDMChannelAsync(this, Kook, options);

    /// <summary>
    ///     获取此私聊频道中具体指定 ID 的用户。
    /// </summary>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <returns> 此私聊频道中具有指定 ID 的用户；如果指定 ID 的用户不存在，或该用户并未参与到此私聊频道中，则返回 <c>null</c>。 </returns>
    public RestUser? GetUser(ulong id)
    {
        if (id == Recipient.Id) return Recipient;
        return id == Kook.CurrentUser?.Id ? CurrentUser : null;
    }

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text, IQuote? quote = null,
        RequestOptions? options = null) =>
        ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.KMarkdown, text, quote, options);

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, path, filename, type, quote, options);

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, stream, filename, type, quote, options);

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, attachment, quote, options);

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectCardsAsync(this, Kook, cards, quote, options);

    /// <inheritdoc />
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectCardAsync(this, Kook, card, quote, options);

    /// <inheritdoc />
    public Task<RestMessage> GetMessageAsync(Guid id, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessageAsync(this, Kook, id, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, null, Direction.Before, limit, true, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, referenceMessageId, dir, limit, true, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        ChannelHelper.GetDirectMessagesAsync(this, Kook, referenceMessage.Id, dir, limit, true, options);

    #endregion

    #region Messages

    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions? options = null) =>
        ChannelHelper.DeleteDirectMessageAsync(this, messageId, Kook, options);

    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null) =>
        ChannelHelper.DeleteDirectMessageAsync(this, message.Id, Kook, options);

    /// <inheritdoc />
    public Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions? options = null) =>
        ChannelHelper.ModifyDirectMessageAsync(this, messageId, func, Kook, options);

    #endregion

    #region IDMChannel

    /// <inheritdoc />
    IUser IDMChannel.Recipient => Recipient;

    #endregion

    #region IRestPrivateChannel

    /// <inheritdoc />
    IReadOnlyCollection<RestUser> IRestPrivateChannel.Recipients => [Recipient];

    #endregion

    #region IPrivateChannel

    /// <inheritdoc />
    IReadOnlyCollection<IUser> IPrivateChannel.Recipients => [Recipient];

    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    async Task<IMessage?> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetMessageAsync(id, options).ConfigureAwait(false)
            : null;

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode,
        RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetMessagesAsync(limit, options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId,
        Direction dir, int limit, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetMessagesAsync(referenceMessageId, dir, limit, options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage,
        Direction dir, int limit, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetMessagesAsync(referenceMessage, dir, limit, options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

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

    /// <summary>
    ///     获取此参与到此私聊频道的另外一位用户的包含 <c>@</c> 前缀的用户名及识别号格式化字符串。
    /// </summary>
    /// <returns> 一个表示此私聊频道的格式化字符串。 </returns>
    /// <seealso cref="Kook.Format.UsernameAndIdentifyNumber(Kook.IUser,System.Boolean)"/>
    public override string ToString() => $"@{Recipient}";

    private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";

    #region IChannel

    /// <inheritdoc />
    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IUser?>(GetUser(id));

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();

    #endregion
}
