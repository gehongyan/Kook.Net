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
    ///     Sends a plain text to this message channel.
    /// </summary>
    /// <param name="text">The message to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Message content is too long, length must be less or equal to <see cref="KookConfig.MaxMessageSize"/>.
    /// </exception>
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, Quote quote = null, RequestOptions options = null)
        => ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Text, text, options, quote: quote);
    /// <summary>
    ///     Sends an image to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends an image as if you are uploading an image directly from your Kook client.
    /// </remarks>
    /// <param name="path">The file path of the image.</param>
    /// <param name="fileName">The name of the image.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Image, createAssetResponse.Url, options, quote: quote);
    }
    /// <summary>
    ///     Sends an image to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends an image as if you are uploading an image directly from your Kook client.
    /// </remarks>
    /// <param name="stream">The stream of the image.</param>
    /// <param name="fileName">The name of the image.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(Stream stream, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Image, createAssetResponse.Url, options, quote: quote);
    }
    /// <summary>
    ///     Sends an image to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends an image as if you are uploading an image directly from your Kook client.
    /// </remarks>
    /// <param name="uri">The URI of the image.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(Uri uri, Quote quote = null, RequestOptions options = null)
    {
        if (!UrlValidation.ValidateKookAssetUrl(uri.OriginalString))
            throw new ArgumentException("The uri cannot be blank.", nameof(uri));
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Image, uri.OriginalString, options, quote: quote);
    }
    /// <summary>
    ///     Sends a video to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends an video as if you are uploading a video directly from your Kook client.
    /// </remarks>
    /// <param name="path">The file path of the video.</param>
    /// <param name="fileName">The name of the video.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Video, createAssetResponse.Url, options, quote: quote);
    }
    /// <summary>
    ///     Sends a video to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends an video as if you are uploading a video directly from your Kook client.
    /// </remarks>
    /// <param name="stream">The stream of the video.</param>
    /// <param name="fileName">The name of the video.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(Stream stream, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Video, createAssetResponse.Url, options, quote: quote);
    }
    /// <summary>
    ///     Sends a video to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends an video as if you are uploading a video directly from your Kook client.
    /// </remarks>
    /// <param name="uri">The URI of the video.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(Uri uri, Quote quote = null, RequestOptions options = null)
    {
        if (!UrlValidation.ValidateKookAssetUrl(uri.OriginalString))
            throw new ArgumentException("The uri cannot be blank.", nameof(uri));
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Video, uri.OriginalString, options, quote: quote);
    }
    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="path">The file path of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.File, createAssetResponse.Url, options, quote: quote);
    }
    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="stream">The stream of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(Stream stream, string fileName = null, Quote quote = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.File, createAssetResponse.Url, options, quote: quote);
    }
    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="uri">The URI of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(Uri uri, Quote quote = null, RequestOptions options = null)
    {
        if (!UrlValidation.ValidateKookAssetUrl(uri.OriginalString))
            throw new ArgumentException("The uri cannot be blank.", nameof(uri));
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.File, uri.OriginalString, options, quote: quote);
    }
    // /// <summary>
    // ///     Sends an audio to this message channel.
    // /// </summary>
    // /// <remarks>
    // ///     This method sends an audio as if you are uploading an audio directly from your Kook client.
    // /// </remarks>
    // /// <param name="path">The file path of the audio.</param>
    // /// <param name="fileName">The name of the audio.</param>
    // /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    // /// <param name="options">The options to be used when sending the request.</param>
    // /// <returns>
    // ///     A task that represents an asynchronous send operation for delivering the message. The task result
    // ///     contains the identifier and timestamp of the sent message.
    // /// </returns>
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(string path, string fileName = null, Quote quote = null, RequestOptions options = null)
    // {
    //     CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), options);
    //     return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Audio, createAssetResponse.Url, options, quote: quote);
    // }
    // /// <summary>
    // ///     Sends an audio to this message channel.
    // /// </summary>
    // /// <remarks>
    // ///     This method sends an audio as if you are uploading an audio directly from your Kook client.
    // /// </remarks>
    // /// <param name="stream">The stream of the audio.</param>
    // /// <param name="fileName">The name of the audio.</param>
    // /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    // /// <param name="options">The options to be used when sending the request.</param>
    // /// <returns>
    // ///     A task that represents an asynchronous send operation for delivering the message. The task result
    // ///     contains the identifier and timestamp of the sent message.
    // /// </returns>
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(Stream stream, string fileName = null, Quote quote = null, RequestOptions options = null)
    // {
    //     CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options);
    //     return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Audio, createAssetResponse.Url, options, quote: quote);
    // }
    // /// <summary>
    // ///     Sends an audio to this message channel.
    // /// </summary>
    // /// <remarks>
    // ///     This method sends an audio as if you are uploading an audio directly from your Kook client.
    // /// </remarks>
    // /// <param name="uri">The URI of the audio.</param>
    // /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    // /// <param name="options">The options to be used when sending the request.</param>
    // /// <returns>
    // ///     A task that represents an asynchronous send operation for delivering the message. The task result
    // ///     contains the identifier and timestamp of the sent message.
    // /// </returns>
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(Uri uri, Quote quote = null, RequestOptions options = null)
    // {
        // if (!UrlValidation.ValidateKookAssetUrl(uri.OriginalString))
        //     throw new ArgumentException("The uri cannot be blank.", nameof(uri));
    //     return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Audio, uri.OriginalString, options, quote: quote);
    // }
    /// <summary>
    ///     Sends a KMarkdown message to this message channel.
    /// </summary>
    /// <param name="text">The message to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, Quote quote = null, RequestOptions options = null)
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
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards, Quote quote = null, RequestOptions options = null)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await ChannelHelper.SendDirectMessageAsync(this, Kook, MessageType.Card, json, options, quote: quote);
    }
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
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(ICard card, Quote quote = null, RequestOptions options = null) => 
        SendCardMessageAsync(new[] { card }, quote, options);
    
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
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendTextMessageAsync(string text,
        
        IQuote quote, RequestOptions options)
        => SendTextMessageAsync(text, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendImageMessageAsync(string path, string fileName,
        IQuote quote, RequestOptions options)
        => SendImageMessageAsync(path, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendImageMessageAsync(Stream stream, string fileName,
        IQuote quote, RequestOptions options)
        => SendImageMessageAsync(stream, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendImageMessageAsync(Uri uri,
        IQuote quote, RequestOptions options)
        => SendImageMessageAsync(uri, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendVideoMessageAsync(string path, string fileName,
        IQuote quote, RequestOptions options)
        => SendVideoMessageAsync(path, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendVideoMessageAsync(Stream stream, string fileName,
        IQuote quote, RequestOptions options)
        => SendVideoMessageAsync(stream, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendVideoMessageAsync(Uri uri,
        IQuote quote, RequestOptions options)
        => SendVideoMessageAsync(uri, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendFileMessageAsync(string path, string fileName,
        IQuote quote, RequestOptions options)
        => SendFileMessageAsync(path, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendFileMessageAsync(Stream stream, string fileName,
        IQuote quote, RequestOptions options)
        => SendFileMessageAsync(stream, fileName, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendFileMessageAsync(Uri uri,
        IQuote quote, RequestOptions options)
        => SendFileMessageAsync(uri, (Quote) quote, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendAudioMessageAsync(string path, string fileName,
    //     IQuote quote, RequestOptions options)
    //     => SendAudioMessageAsync(path, fileName, (Quote) quote, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendAudioMessageAsync(Stream stream, string fileName,
    //     IQuote quote, RequestOptions options)
    //     => SendAudioMessageAsync(stream, fileName, (Quote) quote, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendAudioMessageAsync(Uri uri,
    //     IQuote quote, RequestOptions options)
    //     => SendAudioMessageAsync(uri, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendKMarkdownMessageAsync(string text,
        IQuote quote, RequestOptions options)
        => SendKMarkdownMessageAsync(text, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendCardMessageAsync(ICard card,
        IQuote quote, RequestOptions options)
        => SendCardMessageAsync(card, (Quote) quote, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IDMChannel.SendCardMessageAsync(IEnumerable<ICard> cards,
        IQuote quote, RequestOptions options)
        => SendCardMessageAsync(cards, (Quote) quote, options);
    
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