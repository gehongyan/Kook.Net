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
        : base(kook, default(ulong))
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
    public override Task UpdateAsync(RequestOptions? options = null)
        => SocketChannelHelper.UpdateAsync(this, options);

    /// <inheritdoc />
    public Task CloseAsync(RequestOptions? options = null)
        => ChannelHelper.DeleteDMChannelAsync(this, Kook, options);

    #endregion

    #region Messages

    /// <inheritdoc />
    public SocketMessage GetCachedMessage(Guid id)
        => null;

    /// <summary>
    ///     Gets the message associated with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">TThe ID of the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
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
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null)
        => ChannelHelper.GetDirectMessagesAsync(this, Kook, null, Direction.Before, limit, true, options);

    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(Guid, Direction, int, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="referenceMessageId">The ID of the starting message to get the messages from.</param>
    /// <param name="dir">The direction of the messages to be gotten from.</param>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null)
        => ChannelHelper.GetDirectMessagesAsync(this, Kook, referenceMessageId, dir, limit, true, options);

    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(IMessage, Direction, int, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="referenceMessage">The starting message to get the messages from.</param>
    /// <param name="dir">The direction of the messages to be gotten from.</param>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null)
        => ChannelHelper.GetDirectMessagesAsync(this, Kook, referenceMessage.Id, dir, limit, true, options);

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = KookConfig.MaxMessagesPerBatch)
        => ImmutableArray.Create<SocketMessage>();

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(Guid fromMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch)
        => ImmutableArray.Create<SocketMessage>();

    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch)
        => ImmutableArray.Create<SocketMessage>();

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="path">The file path of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="type">The type of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string fileName = null, AttachmentType type = AttachmentType.File,
        IQuote quote = null, RequestOptions? options = null)
        => ChannelHelper.SendDirectFileAsync(this, Kook, path, fileName, type, options, quote);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="stream">The stream of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="type">The type of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string fileName = null, AttachmentType type = AttachmentType.File,
        IQuote quote = null, RequestOptions? options = null)
        => ChannelHelper.SendDirectFileAsync(this, Kook, stream, fileName, type, options, quote);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="attachment">The attachment containing the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment, IQuote quote = null, RequestOptions? options = null)
        => ChannelHelper.SendDirectFileAsync(this, Kook, attachment, options, quote);

    /// <summary>
    ///     Sends a text message to this message channel.
    /// </summary>
    /// <param name="text">The message to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text, IQuote quote = null, RequestOptions? options = null)
        => ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.KMarkdown, text, options, quote);

    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <param name="cards">The cards to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards, IQuote quote = null, RequestOptions? options = null)
        => ChannelHelper.SendDirectCardsAsync(this, Kook, cards, options, quote);

    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <param name="card">The card to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card, IQuote quote = null, RequestOptions? options = null)
        => ChannelHelper.SendDirectCardAsync(this, Kook, card, options, quote);

    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions? options = null)
        => await ChannelHelper.ModifyDirectMessageAsync(this, messageId, func, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions? options = null)
        => ChannelHelper.DeleteDirectMessageAsync(this, messageId, Kook, options);

    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions? options = null)
        => ChannelHelper.DeleteDirectMessageAsync(this, message.Id, Kook, options);

    internal void AddMessage(SocketMessage msg)
    {
    }

    internal SocketMessage RemoveMessage(Guid id)
        => null;

    #endregion

    #region Users

    /// <summary>
    ///     Gets a user in this channel from the provided <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>
    ///     A <see cref="SocketUser"/> object that is a recipient of this channel; otherwise <c>null</c>.
    /// </returns>
    public new SocketUser GetUser(ulong id)
    {
        if (id == Recipient.Id)
            return Recipient;
        else if (id == Kook.CurrentUser.Id)
            return Kook.CurrentUser;
        else
            return null;
    }

    #endregion

    #region SocketChannel

    /// <inheritdoc />
    internal override void Update(ClientState state, Channel model) =>
        throw new NotSupportedException("Update a DMChannel via Channel is not supported");

    /// <inheritdoc />
    internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;

    /// <inheritdoc />
    internal override SocketUser GetUserInternal(ulong id) => GetUser(id);

    #endregion

    #region IDMChannel

    /// <inheritdoc />
    IUser IDMChannel.Recipient => Recipient;

    #endregion

    #region ISocketPrivateChannel

    /// <inheritdoc />
    IReadOnlyCollection<SocketUser> ISocketPrivateChannel.Recipients => ImmutableArray.Create(Recipient);

    #endregion

    #region IPrivateChannel

    /// <inheritdoc />
    IReadOnlyCollection<IUser> IPrivateChannel.Recipients => ImmutableArray.Create<IUser>(Recipient);

    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendFileAsync(string path, string? fileName,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null)
        => SendFileAsync(path, fileName, type, (Quote)quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendFileAsync(Stream stream, string? fileName,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null)
        => SendFileAsync(stream, fileName, type, (Quote)quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendFileAsync(FileAttachment attachment,
        IQuote? quote, RequestOptions? options = null)
        => SendFileAsync(attachment, (Quote)quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendTextAsync(string text,
        IQuote? quote, RequestOptions? options = null)
        => SendTextAsync(text, (Quote)quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendCardAsync(ICard card,
        IQuote? quote, RequestOptions? options = null)
        => SendCardAsync(card, (Quote)quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IDMChannel.SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote, RequestOptions? options = null)
        => SendCardsAsync(cards, (Quote)quote, options);

    /// <inheritdoc />
    async Task<IMessage> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions? options = null)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetMessageAsync(id, options).ConfigureAwait(false);
        else
            return GetCachedMessage(id);
    }

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(limit, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId,
        Direction dir, int limit,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(referenceMessageId, dir, limit, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage,
        Direction dir, int limit,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null)
        => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(referenceMessage.Id, dir, limit, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(string path, string? fileName,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, IUser? ephemeralUser = null,
        RequestOptions? options = null)
        => SendFileAsync(path, fileName, type, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(Stream stream, string? fileName,
        AttachmentType type, IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null)
        => SendFileAsync(stream, fileName, type, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(FileAttachment attachment,
        IQuote? quote, IUser? ephemeralUser = null, RequestOptions? options = null)
        => SendFileAsync(attachment, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendTextAsync(string text,
        IQuote? quote, IUser? ephemeralUser = null, RequestOptions? options = null)
        => SendTextAsync(text, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote, IUser? ephemeralUser = null, RequestOptions? options = null)
        => SendCardsAsync(cards, quote, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardAsync(ICard card,
        IQuote? quote, IUser? ephemeralUser = null, RequestOptions? options = null)
        => SendCardAsync(card, quote, options);

    #endregion

    #region IChannel

    /// <inheritdoc />
    string IChannel.Name => $"@{Recipient}";

    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options = null)
        => Task.FromResult<IUser>(GetUser(id));

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options = null)
        => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();

    #endregion

    /// <summary>
    ///     Returns the recipient user.
    /// </summary>
    public override string ToString() => $"@{Recipient}";

    private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
    internal new SocketDMChannel Clone() => MemberwiseClone() as SocketDMChannel;
}
