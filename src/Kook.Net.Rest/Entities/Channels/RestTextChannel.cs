using Model = Kook.API.Channel;

using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API;
using Kook.API.Rest;
using Kook.Net.Converters;
using Kook.Utils;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based channel in a guild that can send and receive messages.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestTextChannel : RestGuildChannel, IRestMessageChannel, ITextChannel
{
    #region RestTextChannel
    
    /// <inheritdoc />
    public string Topic { get; private set; }
    /// <inheritdoc />
    public virtual int SlowModeInterval { get; private set; }
    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }
    /// <inheritdoc />
    public bool IsPermissionSynced { get; private set; }
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);
    
    internal RestTextChannel(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, guild, id, ChannelType.Text)
    {
    }
    
    internal new static RestTextChannel Create(BaseKookClient kook, IGuild guild, Model model)
    {
        var entity = new RestTextChannel(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }
    /// <inheritdoc />
    internal override void Update(Model model)
    {
        base.Update(model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        Topic = model.Topic;
        SlowModeInterval = model.SlowMode / 1000;
        IsPermissionSynced = model.PermissionSync == 1;
    }
    
    /// <inheritdoc />
    public virtual async Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions options = null)
    {
        var model = await ChannelHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        Update(model);
    }
    
    /// <summary>
    ///     Gets a user in this channel.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="InvalidOperationException">
    /// Resolving permissions requires the parent guild to be downloaded.
    /// </exception>
    /// <returns>
    ///     A task representing the asynchronous get operation. The task result contains a guild user object that
    ///     represents the user; <c>null</c> if none is found.
    /// </returns>
    public Task<RestGuildUser> GetUserAsync(ulong id, RequestOptions options = null)
        => ChannelHelper.GetUserAsync(this, Guild, Kook, id, options);

    /// <summary>
    ///     Gets a collection of users that are able to view the channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="InvalidOperationException">
    /// Resolving permissions requires the parent guild to be downloaded.
    /// </exception>
    /// <returns>
    ///     A paged collection containing a collection of guild users that can access this channel. Flattening the
    ///     paginated response into a collection of users with
    ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the users.
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(RequestOptions options = null)
        => ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options: options);

    /// <inheritdoc />
    public Task<RestMessage> GetMessageAsync(Guid id, RequestOptions options = null)
        => ChannelHelper.GetMessageAsync(this, Kook, id, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, Kook, null, Direction.Before, limit, true, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, Kook, referenceMessageId, dir, limit, true, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, Kook, referenceMessage.Id, dir, limit, true, options);
    public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
        => ChannelHelper.GetPinnedMessagesAsync(this, Kook, options);
    
    /// <inheritdoc cref="IMessageChannel.SendTextMessageAsync(string,IQuote,IUser,RequestOptions)"/>
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendMessageAsync(this, Kook, MessageType.Text, text, options, quote: quote, ephemeralUser: ephemeralUser);
    /// <inheritdoc cref="IMessageChannel.SendImageMessageAsync(string,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Image, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendImageMessageAsync(Stream,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(Stream stream, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options);
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
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.Video, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendVideoMessageAsync(Stream,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(Stream stream, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options);
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
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options);
        return await ChannelHelper.SendMessageAsync(this, Kook, MessageType.File, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    /// <inheritdoc cref="IMessageChannel.SendFileMessageAsync(Stream,string,IQuote,IUser,RequestOptions)"/>
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(Stream stream, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = fileName}, options);
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
    //     CreateAssetResponse createAssetResponse = await Kook.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, fileName, options);
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
    /// <inheritdoc cref="IMessageChannel.SendCardMessageAsync(ICard,IQuote,IUser,RequestOptions)"/>
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(ICard card, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null) => 
        SendCardMessageAsync(new[] { card }, quote, ephemeralUser, options);

    /// <summary>
    ///     Gets the parent (category) channel of this channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the category channel
    ///     representing the parent of this channel; <c>null</c> if none is set.
    /// </returns>
    public Task<ICategoryChannel> GetCategoryAsync(RequestOptions options = null)
        => ChannelHelper.GetCategoryAsync(this, Kook, options);
    /// <inheritdoc />
    public Task SyncPermissionsAsync(RequestOptions options = null)
        => ChannelHelper.SyncPermissionsAsync(this, Kook, options);
    
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

    #region IChannel
    
    /// <inheritdoc />
    async Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetUserAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetUsersAsync(options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
    }

    #endregion
    
    #region IGuildChannel
    
    /// <inheritdoc />
    async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetUserAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
    {
        return mode == CacheMode.AllowDownload
            ? GetUsersAsync(options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
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
    //     => uri fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendKMarkdownMessageAsync(string text,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendKMarkdownMessageAsync(text, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendCardMessageAsync(ICard card,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendCardMessageAsync(card, (Quote) quote, ephemeralUser, options);
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
            return null;
    }
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetMessagesAsync(limit, options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
    }

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetMessagesAsync(referenceMessageId, dir, limit, options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
    }
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetMessagesAsync(referenceMessage, dir, limit, options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
    }
    /// <inheritdoc />
    async Task<IReadOnlyCollection<IMessage>> ITextChannel.GetPinnedMessagesAsync(RequestOptions options)
        => await GetPinnedMessagesAsync(options).ConfigureAwait(false);
    
    /// <inheritdoc />
    public Task DeleteMessageAsync(Guid messageId, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, messageId, Kook, options);
    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, message.Id, Kook, options);
    
    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null)
        => await ChannelHelper.ModifyMessageAsync(this, messageId, func, Kook, options).ConfigureAwait(false);

    #endregion
    
    #region INestedChannel
    
    /// <inheritdoc />
    async Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
    {
        if (CategoryId.HasValue && mode == CacheMode.AllowDownload)
            return (await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false)) as ICategoryChannel;
        return null;
    }
    
    #endregion
}