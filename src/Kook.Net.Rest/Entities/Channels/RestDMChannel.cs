using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.UserChat;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based direct-message channel.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestDMChannel : RestChannel, IDMChannel, IRestPrivateChannel, IRestMessageChannel
{
    #region RestDMChannel

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
    ///     Gets the current logged-in user.
    /// </summary>
    public RestUser CurrentUser { get; }

    /// <summary>
    ///     Gets the recipient of the channel.
    /// </summary>
    public RestUser Recipient { get; }

    /// <summary>
    ///     Gets a collection that is the current logged-in user and the recipient.
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
    ///     Gets a user in this channel from the provided <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>
    ///     A <see cref="RestUser"/> object that is a recipient of this channel; otherwise <c>null</c>.
    /// </returns>
    public RestUser? GetUser(ulong id)
    {
        if (id == Recipient.Id) return Recipient;
        return id == Kook.CurrentUser?.Id ? CurrentUser : null;
    }

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
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text, IQuote? quote = null,
        RequestOptions? options = null) =>
        ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.KMarkdown, text, quote, options);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="path">The file path of the file.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="type">The type of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, path, filename, type, quote, options);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="stream">The stream of the file.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="type">The type of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, stream, filename, type, quote, options);

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
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectFileAsync(this, Kook, attachment, quote, options);

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
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote = null, RequestOptions? options = null) =>
        ChannelHelper.SendDirectCardsAsync(this, Kook, cards, quote, options);

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
    ///     Gets a string that represents the Username#IdentifyNumber of the recipient.
    /// </summary>
    /// <returns>
    ///     A string that resolves to the Recipient of this channel.
    /// </returns>
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
