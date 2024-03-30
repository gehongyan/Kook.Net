using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.Channel;

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
    public string Topic { get; private set; }

    /// <inheritdoc />
    public int SlowModeInterval { get; private set; }

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
    public bool? IsPermissionSynced { get; private set; }

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    /// <inheritdoc />
    /// <remarks>
    ///     This property is only available if the <see cref="KookSocketConfig.MessageCacheSize"/> is set to a value greater than zero.
    /// </remarks>
    /// <seealso cref="KookSocketConfig.MessageCacheSize"/>
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

    internal static new SocketTextChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        SocketTextChannel entity = new(guild.Kook, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }

    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        Topic = model.Topic;
        SlowModeInterval = model.SlowMode / 1000;
        IsPermissionSynced = model.PermissionSync;
    }

    /// <inheritdoc />
    public virtual Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions options = null)
        => ChannelHelper.ModifyAsync(this, Kook, func, options);

    /// <inheritdoc />
    public virtual Task SyncPermissionsAsync(RequestOptions options = null)
        => ChannelHelper.SyncPermissionsAsync(this, Kook, options);

    private string DebuggerDisplay => $"{Name} ({Id}, Text)";
    internal new SocketTextChannel Clone() => MemberwiseClone() as SocketTextChannel;

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
        if (msg == null) msg = await ChannelHelper.GetMessageAsync(this, Kook, id, options).ConfigureAwait(false);

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
    public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
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
    public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
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
    public virtual IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions options = null)
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
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800, InviteMaxUses maxUses = InviteMaxUses.Unlimited,
        RequestOptions options = null)
        => await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    #region Users

    /// <inheritdoc />
    public override SocketGuildUser GetUser(ulong id)
    {
        SocketGuildUser user = Guild.GetUser(id);
        if (user != null)
        {
            ulong guildPerms = Permissions.ResolveGuild(Guild, user);
            ulong channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
            if (Permissions.GetValue(channelPerms, ChannelPermission.ViewChannel)) return user;
        }

        return null;
    }

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        SocketGuildUser user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly) return user;

        return await ChannelHelper.GetUserAsync(this, Guild, Kook, id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options) =>
        mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();

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
            return GetCachedMessage(id);
    }

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit, CacheMode mode, RequestOptions options)
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, null, Direction.Before, limit, mode, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit,
        CacheMode mode, RequestOptions options)
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, referenceMessageId, dir, limit, mode, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit,
        CacheMode mode, RequestOptions options)
        => SocketChannelHelper.GetMessagesAsync(this, Kook, _messages, referenceMessage.Id, dir, limit, mode, options);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IMessage>> ITextChannel.GetPinnedMessagesAsync(RequestOptions options)
        => await GetPinnedMessagesAsync(options).ConfigureAwait(false);

    #endregion

    #region INestedChannel

    /// <inheritdoc />
    Task<ICategoryChannel> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult(Category);

    #endregion
}
