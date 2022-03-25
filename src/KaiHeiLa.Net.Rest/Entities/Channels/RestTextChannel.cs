using Model = KaiHeiLa.API.Channel;

using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using KaiHeiLa.API;
using KaiHeiLa.API.Rest;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.Rest;

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
    public ulong CreatorId { get; private set; }
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);
    
    internal RestTextChannel(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, ulong id)
        : base(kaiHeiLa, guild, id, ChannelType.Text)
    {
    }
    
    internal new static RestTextChannel Create(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, Model model)
    {
        var entity = new RestTextChannel(kaiHeiLa, guild, model.Id);
        entity.Update(model);
        return entity;
    }
    /// <inheritdoc />
    internal override void Update(Model model)
    {
        base.Update(model);
        CategoryId = model.CategoryId;
        Topic = model.Topic;
        SlowModeInterval = model.SlowMode;
        IsPermissionSynced = model.PermissionSync == 1;
        CreatorId = model.CreatorId;
    }
    
    /// <summary>
    ///     Gets a user in this channel.
    /// </summary>
    /// <param name="id">The snowflake identifier of the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="InvalidOperationException">
    /// Resolving permissions requires the parent guild to be downloaded.
    /// </exception>
    /// <returns>
    ///     A task representing the asynchronous get operation. The task result contains a guild user object that
    ///     represents the user; <c>null</c> if none is found.
    /// </returns>
    public Task<RestGuildUser> GetUserAsync(ulong id, RequestOptions options = null)
        => ChannelHelper.GetUserAsync(this, Guild, KaiHeiLa, id, options);

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
        => ChannelHelper.GetUsersAsync(this, Guild, KaiHeiLa, KaiHeiLaConfig.MaxUsersPerBatch, 1, options: options);

    /// <summary>
    ///     Gets the creator of this channel.
    /// </summary>
    /// <remarks>
    ///     This method will try to get the user as a global user. To get the creator as a guild member,
    ///     you will need to get the user through
    ///     <see cref="IGuild.GetUserAsync(ulong,CacheMode,RequestOptions)"/>."/>
    /// </remarks>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the creator of this channel.
    /// </returns>
    public Task<RestUser> GetCreatorAsync(RequestOptions options = null)
        => ClientHelper.GetUserAsync(KaiHeiLa, CreatorId, options);

    /// <inheritdoc />
    public Task<RestMessage> GetMessageAsync(Guid id, RequestOptions options = null)
        => ChannelHelper.GetMessageAsync(this, KaiHeiLa, id, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, KaiHeiLa, null, Direction.Before, limit, true, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, KaiHeiLa, referenceMessageId, dir, limit, true, options);
    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, KaiHeiLa, referenceMessage.Id, dir, limit, true, options);
    public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
        => ChannelHelper.GetPinnedMessagesAsync(this, KaiHeiLa, options);
    
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Text, text, options, quote: quote, ephemeralUser: ephemeralUser);
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options);
        return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Image, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options);
        return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Video, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(new CreateAssetParams {File = File.OpenRead(path), FileName = fileName}, options);
        return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.File, createAssetResponse.Url, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
    // public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(string path, string fileName = null, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    // {
    //     CreateAssetResponse createAssetResponse = await KaiHeiLa.ApiClient.CreateAssetAsync(path, fileName, options);
    //     return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Audio, createAssetResponse.Url, options, quote: quote,
    //         ephemeralUser: ephemeralUser);
    // }
    public Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.KMarkdown, text, options, quote: quote, ephemeralUser: ephemeralUser);

    public async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards, Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await ChannelHelper.SendMessageAsync(this, KaiHeiLa, MessageType.Card, json, options, quote: quote,
            ephemeralUser: ephemeralUser);
    }
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
        => ChannelHelper.GetCategoryAsync(this, KaiHeiLa, options);
    
    #endregion

    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions options = null)
        => await ChannelHelper.GetInvitesAsync(this, KaiHeiLa, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null, RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, KaiHeiLa, maxAge, maxUses, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge.OneWeek, InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, KaiHeiLa, maxAge, maxUses, options).ConfigureAwait(false);

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
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendVideoMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendVideoMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    /// <inheritdoc />
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendFileMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
    // /// <inheritdoc />
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendAudioMessageAsync(string path, string fileName = null,
    //     IQuote quote, IUser ephemeralUser, RequestOptions options)
    //     => SendAudioMessageAsync(path, fileName, (Quote) quote, ephemeralUser, options);
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
        => ChannelHelper.DeleteMessageAsync(this, messageId, KaiHeiLa, options);
    /// <inheritdoc />
    public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        => ChannelHelper.DeleteMessageAsync(this, message.Id, KaiHeiLa, options);
    
    /// <inheritdoc />
    public async Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null)
        => await ChannelHelper.ModifyMessageAsync(this, messageId, func, KaiHeiLa, options).ConfigureAwait(false);

    #endregion
    
    #region INestedChannel
    
    /// <inheritdoc />
    async Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
    {
        if (CategoryId.HasValue && mode == CacheMode.AllowDownload)
            return (await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false)) as ICategoryChannel;
        return null;
    }
    /// <inheritdoc />
    async Task<IUser> INestedChannel.GetCreatorAsync(CacheMode mode, RequestOptions options = null)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetCreatorAsync(options).ConfigureAwait(false);
        else
            return null;
    }
    
    #endregion
}