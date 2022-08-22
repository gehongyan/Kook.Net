using System.Collections.Immutable;
using Model = Kook.API.Channel;

using System.Diagnostics;
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
///     Represents a WebSocket-based channel in a guild that can send and receive messages.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketTextChannel : SocketGuildChannel, ITextChannel, ISocketMessageChannel
{
    #region SocketTextChannel

    private readonly MessageCache _messages;
    
    /// <inheritdoc />
    public string Topic { get; set; }
    /// <inheritdoc />
    public int SlowModeInterval { get; set; }
    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }
    /// <summary>
    ///     Gets the parent (category) of this channel in the guild's channel list.
    /// </summary>
    /// <returns>
    ///     An <see cref="ICategoryChannel"/> representing the parent of this channel; <c>null</c> if none is set.
    /// </returns>
    public ICategoryChannel Category
        => CategoryId.HasValue ? Guild.GetChannel(CategoryId.Value) as ICategoryChannel : null;
    /// <inheritdoc />
    public bool IsPermissionSynced { get; private set; }
    /// <inheritdoc />
    public ulong CreatorId { get; private set; }
    /// <summary>
    ///     Gets the creator of this channel.
    /// </summary>
    /// <remarks>
    ///     This method will try to get the user as a member of this channel. If the user is not a member of this guild,
    ///     this method will return <c>null</c>. To get the creator under this circumstance, use
    ///     <see cref="Kook.Rest.KookRestClient.GetUserAsync(ulong,RequestOptions)"/>.
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the creator of this channel.
    /// </returns>
    public SocketGuildUser Creator => GetUser(CreatorId);
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    public IReadOnlyCollection<SocketMessage> CachedMessages => _messages?.Messages ?? ImmutableArray.Create<SocketMessage>();
    
    /// <inheritdoc />
    public override IReadOnlyCollection<SocketGuildUser> Users
        => Guild.Users.Where(x => Permissions.GetValue(
            Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
            ChannelPermission.ViewChannel)).ToImmutableArray();

    internal SocketTextChannel(KookSocketClient kook, ulong id, SocketGuild guild)
        : base(kook, id, guild)
    {
        Type = ChannelType.Text;
        if (Kook.MessageCacheSize > 0)
            _messages = new MessageCache(Kook);
    }
    internal new static SocketTextChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketTextChannel(guild.Kook, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }
    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        Topic = model.Topic;
        SlowModeInterval = model.SlowMode / 1000;
        IsPermissionSynced = model.PermissionSync == 1;
        CreatorId = model.CreatorId;
    }
    
    /// <inheritdoc />
    public virtual Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions options = null)
        => ChannelHelper.ModifyAsync(this, Kook, func, options);
    
    #endregion

    #region Messages
    
    /// <inheritdoc />
    public SocketMessage GetCachedMessage(Guid id)
        => _messages?.Get(id);
    /// <summary>
    ///     Gets a message from this message channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessageAsync"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="id">The identifier of the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
    ///     the retrieved message; <c>null</c> if no message is found with the specified identifier.
    /// </returns>
    public async Task<IMessage> GetMessageAsync(Guid id, RequestOptions options = null)
    {
        IMessage msg = _messages?.Get(id);
        if (msg == null)
            msg = await ChannelHelper.GetMessageAsync(this, Kook, id, options).ConfigureAwait(false);
        return msg;
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
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, null, Direction.Before, limit, CacheMode.AllowDownload, options);
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
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, referenceMessageId, dir, limit, CacheMode.AllowDownload, options);
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
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, referenceMessage.Id, dir, limit, CacheMode.AllowDownload, options);
    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = KookConfig.MaxMessagesPerBatch)
        => SocketChannelHelper.GetCachedMessages(this, Kook, _messages, null, Direction.Before, limit);
    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(Guid fromMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch)
        => SocketChannelHelper.GetCachedMessages(this, Kook, _messages, fromMessageId, dir, limit);
    /// <inheritdoc />
    public IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch)
        => SocketChannelHelper.GetCachedMessages(this, Kook, _messages, fromMessage.Id, dir, limit);
    /// <inheritdoc cref="ITextChannel.GetPinnedMessagesAsync(RequestOptions)"/>
    public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
        => ChannelHelper.GetPinnedMessagesAsync(this, Kook, options);
    /// <inheritdoc cref="IMessageChannel.SendTextMessageAsync(string,IQuote,IUser,RequestOptions)"/>
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendMessageAsync(this, Kook, MessageType.Text, text, options, quote: quote, ephemeralUser: ephemeralUser);
    /// <inheritdoc cref="IMessageChannel.SendImageMessageAsync(string,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Image, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendImageMessageAsync(Stream,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(Stream stream, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Image, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendImageMessageAsync(Uri,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(Uri uri, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        if (!UrlValidation.ValidateKookAssetUrl(uri.OriginalString))
            throw new ArgumentException("The uri cannot be blank.", nameof(uri));
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Image, uri.OriginalString, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendVideoMessageAsync(string,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Video, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendVideoMessageAsync(Stream,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(Stream stream, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Video, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendVideoMessageAsync(Uri,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(Uri uri, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        if (!UrlValidation.ValidateKookAssetUrl(uri.OriginalString))
            throw new ArgumentException("The uri cannot be blank.", nameof(uri));
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Video, uri.OriginalString, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendFileMessageAsync(string,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.File, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendFileMessageAsync(Stream,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(Stream stream, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options).ConfigureAwait(false);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.File, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendFileMessageAsync(Uri,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(Uri uri, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        if (!UrlValidation.ValidateKookAssetUrl(uri.OriginalString))
            throw new ArgumentException("The uri cannot be blank.", nameof(uri));
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.File, uri.OriginalString, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    // /// <inheritdoc cref="IMessageChannel.SendAudioMessageAsync(string,string,IQuote,IUser,RequestOptions)"/>
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    // {
    //     CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options);
    //     return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Audio, createAssetResponse.Url, options, quote: quote,
    //         ephemeralUser: ephemeralUser);
    // }
    // /// <inheritdoc cref="IMessageChannel.SendAudioMessageAsync(Stream,string,IQuote,IUser,RequestOptions)"/>
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(Stream stream, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    // {
    //     CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options);
    //     return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Audio, createAssetResponse.Url, options, quote: quote,
    //         ephemeralUser: ephemeralUser);
    // }
    // /// <inheritdoc cref="IMessageChannel.SendAudioMessageAsync(Uri,IQuote,IUser,RequestOptions)"/>
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(Uri uri, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    // {
    //     if (!UrlValidation.ValidateKookAssetUrl(uri.OriginalString))
    //         throw new ArgumentException("The uri cannot be blank.", nameof(uri));
    //     return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Audio, uri.OriginalString, options, quote: quote,
    //         ephemeralUser: ephemeralUser);
    // }
    /// <inheritdoc cref="IMessageChannel.SendKMarkdownMessageAsync(string,IQuote,IUser,RequestOptions)"/>
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendMessageAsync(this, Kook, MessageType.KMarkdown, text, options, quote: quote, ephemeralUser: ephemeralUser);
    /// <inheritdoc cref="IMessageChannel.SendCardMessageAsync(IEnumerable{ICard},IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Card, json, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <param name="card">The card to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="ephemeralUser">The user only who can see the message. Leave null to let everyone see the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(ICard card, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null) => 
        SendCardMessageAsync(new[] { card }, quote, ephemeralUser, options);

    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null)
        => await ChannelHelper.ModifyMessageAsync(this, messageId, func, Kook, options).ConfigureAwait(false);
    
    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, messageId, Kook, options);
    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, message.Id, Kook, options);
    
    internal void AddMessage(SocketMessage msg)
        => _messages?.Add(msg);
    internal SocketMessage RemoveMessage(Guid id)
        => _messages?.Remove(id);
    
    #endregion

    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions options = null)
        => await ChannelHelper.GetInvitesAsync(this, Kook, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null, RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800, InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion
    
    private string DebuggerDisplay => $"{Name} ({Id}, Text)";
    internal new SocketTextChannel Clone() => MemberwiseClone() as SocketTextChannel;

    #region Users
    /// <inheritdoc />
    public override SocketGuildUser GetUser(ulong id)
    {
        var user = Guild.GetUser(id);
        if (user != null)
        {
            var guildPerms = Permissions.ResolveGuild(Guild, user);
            var channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
            if (Permissions.GetValue(channelPerms, ChannelPermission.ViewChannel))
                return user;
        }
        return null;
    }
    #endregion
    
    #region IGuildChannel
    /// <inheritdoc />
    async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        var user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly)
            return user;

        return await ChannelHelper.GetUserAsync(this, Guild, Kook, id, options).ConfigureAwait(false);
    }
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
    {
        return mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
    }

    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendTextMessageAsync(string text,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendTextMessageAsync(text, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendImageMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendImageMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendImageMessageAsync(Stream stream, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendImageMessageAsync(stream, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendImageMessageAsync(Uri uri,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendImageMessageAsync(uri, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendVideoMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendVideoMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendVideoMessageAsync(Stream stream, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendVideoMessageAsync(stream, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendVideoMessageAsync(Uri uri,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendVideoMessageAsync(uri, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendFileMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendFileMessageAsync(Stream stream, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileMessageAsync(stream, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendFileMessageAsync(Uri uri,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileMessageAsync(uri, (Quote) quote, ephemeralUser, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendAudioMessageAsync(string path, string fileName,
    //     IQuote quote, IUser ephemeralUser, RequestOptions options)
    //     => SendAudioMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendAudioMessageAsync(Stream stream, string fileName,
    //     IQuote quote, IUser ephemeralUser, RequestOptions options)
    //     => SendAudioMessageAsync(stream, fileName, (Quote) quote, ephemeralUser, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendAudioMessageAsync(Uri uri,
    //     IQuote quote, IUser ephemeralUser, RequestOptions options)
    //     => SendAudioMessageAsyncuri, , (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendKMarkdownMessageAsync(string text,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendKMarkdownMessageAsync(text, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendCardMessageAsync(IEnumerable<ICard> cards,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendCardMessageAsync(cards, (Quote) quote, ephemeralUser, options);
    
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
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, null, Direction.Before, limit, mode, options);
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, referenceMessageId, dir, limit, mode, options);
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, referenceMessage.Id, dir, limit, mode, options);
    /// <inheritdoc />
    async Task<IReadOnlyCollection<IMessage>> ITextChannel.GetPinnedMessagesAsync(RequestOptions options)
        => await GetPinnedMessagesAsync(options).ConfigureAwait(false);
    #endregion
    
    #region  INestedChannel
    
    /// <inheritdoc />
    Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult(Category);
    
    /// <inheritdoc />
    /// <remarks>
    ///     This method will try to get the user as a member of this channel. If the user is not a member of this guild,
    ///     this method will return <c>null</c>. To get the creator under this circumstance, use
    ///     <see cref="Kook.Rest.KookRestClient.GetUserAsync(ulong,RequestOptions)"/>.
    /// </remarks>
    Task<IUser> INestedChannel.GetCreatorAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(Creator);
    
    #endregion
}