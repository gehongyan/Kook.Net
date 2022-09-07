using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API;
using Kook.API.Rest;
using Kook.Net.Converters;
using Kook.Rest;
using Kook.Utils;

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

    internal static SocketDMChannel Create(KookSocketClient kook, ClientState state, Guid chatCode, API.User recipient)
    {
        var entity = new SocketDMChannel(kook, chatCode, kook.GetOrCreateTemporaryUser(state, recipient));
        entity.Update(state, recipient);
        return entity;
    }
    internal void Update(ClientState state, API.User recipient)
    {
        Recipient.Update(state, recipient);
        Recipient.UpdatePresence(recipient.Online, recipient.OperatingSystem);
    }
    internal void Update(ClientState state, UserChat model)
    {
        Recipient.Update(state, model.Recipient);
    }

    /// <inheritdoc />
    public override Task UpdateAsync(RequestOptions options = null)
        => SocketChannelHelper.UpdateAsync(this, options);
    
    /// <inheritdoc />
    public Task CloseAsync(RequestOptions options = null)
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
    public async Task<IMessage> GetMessageAsync(Guid id, RequestOptions options = null)
    {
        return await ChannelHelper.GetDirectMessageAsync(this, Kook, id, options).ConfigureAwait(false);
    }
    
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
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
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
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
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
    public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
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
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileAsync(string path, string fileName = null, AttachmentType type = AttachmentType.File, Quote quote = null, RequestOptions options = null)
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
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileAsync(Stream stream, string fileName = null, AttachmentType type = AttachmentType.File, Quote quote = null, RequestOptions options = null)
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
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileAsync(FileAttachment attachment, Quote quote = null, RequestOptions options = null)
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
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextAsync(string text, Quote quote = null, RequestOptions options = null)
        => ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.KMarkdown, text, options, quote: quote);
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
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardsAsync(IEnumerable<ICard> cards, Quote quote = null, RequestOptions options = null)
        => ChannelHelper.SendDirectCardsAsync(this, Kook, cards, options, quote: quote);
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
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardAsync(ICard card, Quote quote = null, RequestOptions options = null)
        => ChannelHelper.SendDirectCardAsync(this, Kook, card, options, quote: quote);
    
    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null)
        => await ChannelHelper.ModifyDirectMessageAsync(this, messageId, func, Kook, options).ConfigureAwait(false);
    
    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions options = null)
        => ChannelHelper.DeleteDirectMessageAsync(this, messageId, Kook, options);
    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
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
    internal override void Update(ClientState state, Channel model)
    {
        throw new NotSupportedException("Update a DMChannel via Channel is not supported");
    }
    
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
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendFileAsync(string path, string fileName,
        AttachmentType type, IQuote quote, RequestOptions options)
        => SendFileAsync(path, fileName, type, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendFileAsync(Stream stream, string fileName,
        AttachmentType type, IQuote quote, RequestOptions options)
        => SendFileAsync(stream, fileName, type, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendFileAsync(FileAttachment attachment,
        IQuote quote, RequestOptions options)
        => SendFileAsync(attachment, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendTextAsync(string text,
        IQuote quote, RequestOptions options)
        => SendTextAsync(text, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendCardAsync(ICard card,
        IQuote quote, RequestOptions options)
        => SendCardAsync(card, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendCardsAsync(IEnumerable<ICard> cards,
        IQuote quote, RequestOptions options)
        => SendCardsAsync(cards, (Quote) quote, options);
    
    /// <inheritdoc />
    async Task<IMessage> IMessageChannel.GetMessageAsync(Guid id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetMessageAsync(id, options).ConfigureAwait(false);
        else
            return GetCachedMessage(id);
    }
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
        => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(limit, options);
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
        => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(referenceMessageId, dir, limit, options);
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
        => mode == CacheMode.CacheOnly ? null : GetMessagesAsync(referenceMessage.Id, dir, limit, options);
    
    #endregion
    
    #region IChannel
    
    /// <inheritdoc />
    string IChannel.Name => $"@{Recipient}";

    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(GetUser(id));
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();
    
    #endregion
    
    /// <summary>
    ///     Returns the recipient user.
    /// </summary>
    public override string ToString() => $"@{Recipient}";
    private string DebuggerDisplay => $"@{Recipient} ({Id}, DM)";
    internal new SocketDMChannel Clone() => MemberwiseClone() as SocketDMChannel;
}