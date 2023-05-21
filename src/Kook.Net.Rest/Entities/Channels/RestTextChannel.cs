using System.Diagnostics;
using Model = Kook.API.Channel;

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
    public bool? IsPermissionSynced { get; private set; }

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    internal RestTextChannel(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, guild, id, ChannelType.Text)
    {
    }

    internal static new RestTextChannel Create(BaseKookClient kook, IGuild guild, Model model)
    {
        RestTextChannel entity = new(kook, guild, model.Id);
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
        IsPermissionSynced = model.PermissionSync;
    }

    /// <inheritdoc />
    public virtual async Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions options = null)
    {
        Model model = await ChannelHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
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
        => ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options);

    /// <inheritdoc />
    public Task<RestMessage> GetMessageAsync(Guid id, RequestOptions options = null)
        => ChannelHelper.GetMessageAsync(this, Kook, id, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = KookConfig.MaxMessagesPerBatch,
        RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, Kook, null, Direction.Before, limit, true, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, Kook, referenceMessageId, dir, limit, true, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
        => ChannelHelper.GetMessagesAsync(this, Kook, referenceMessage.Id, dir, limit, true, options);

    /// <inheritdoc cref="ITextChannel.GetPinnedMessagesAsync" />
    public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
        => ChannelHelper.GetPinnedMessagesAsync(this, Kook, options);

    /// <inheritdoc cref="IMessageChannel.SendFileAsync(string,string,AttachmentType,IQuote,IUser,RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string fileName = null, AttachmentType type = AttachmentType.File,
        Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendFileAsync(this, Kook, path, fileName, type, options, quote, ephemeralUser);

    /// <inheritdoc cref="IMessageChannel.SendFileAsync(Stream,string,AttachmentType,IQuote,IUser,RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string fileName = null, AttachmentType type = AttachmentType.File,
        Quote quote = null, IUser ephemeralUser = null, RequestOptions options = null)
        => ChannelHelper.SendFileAsync(this, Kook, stream, fileName, type, options, quote, ephemeralUser);

    /// <inheritdoc cref="IMessageChannel.SendFileAsync(FileAttachment,IQuote,IUser,RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment, Quote quote = null, IUser ephemeralUser = null,
        RequestOptions options = null)
        => ChannelHelper.SendFileAsync(this, Kook, attachment, options, quote, ephemeralUser);

    /// <inheritdoc cref="IMessageChannel.SendTextAsync(string,IQuote,IUser,RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text, Quote quote = null, IUser ephemeralUser = null,
        RequestOptions options = null)
        => ChannelHelper.SendMessageAsync(this, Kook, MessageType.KMarkdown, text, options, quote, ephemeralUser);

    /// <inheritdoc cref="IMessageChannel.SendCardsAsync(IEnumerable{ICard},IQuote,IUser,RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards, Quote quote = null, IUser ephemeralUser = null,
        RequestOptions options = null)
        => ChannelHelper.SendCardsAsync(this, Kook, cards, options, quote, ephemeralUser);

    /// <inheritdoc cref="IMessageChannel.SendCardAsync(ICard,IQuote,IUser,RequestOptions)"/>
    public Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card, Quote quote = null, IUser ephemeralUser = null,
        RequestOptions options = null)
        => ChannelHelper.SendCardAsync(this, Kook, card, options, quote, ephemeralUser);

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
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800, InviteMaxUses maxUses = InviteMaxUses.Unlimited,
        RequestOptions options = null)
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
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options) =>
        mode == CacheMode.AllowDownload
            ? GetUsersAsync(options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    #endregion

    #region IMessageChannel

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(string path, string fileName,
        AttachmentType type, IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileAsync(path, fileName, type, (Quote)quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(Stream stream, string fileName,
        AttachmentType type, IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileAsync(stream, fileName, type, (Quote)quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendFileAsync(FileAttachment attachment,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileAsync(attachment, (Quote)quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendTextAsync(string text,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendTextAsync(text, (Quote)quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardAsync(ICard card,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendCardAsync(card, (Quote)quote, ephemeralUser, options);

    /// <inheritdoc />
    Task<Cacheable<IUserMessage, Guid>> IMessageChannel.SendCardsAsync(IEnumerable<ICard> cards,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendCardsAsync(cards, (Quote)quote, ephemeralUser, options);

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
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit,
        CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetMessagesAsync(referenceMessageId, dir, limit, options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();
    }

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit,
        CacheMode mode, RequestOptions options)
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
            return await Guild.GetChannelAsync(CategoryId.Value, mode, options).ConfigureAwait(false) as ICategoryChannel;

        return null;
    }

    #endregion
}
