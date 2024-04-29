using Kook.API.Gateway;
using Kook.Rest;
using System.Collections.Immutable;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based message.
/// </summary>
public abstract class SocketMessage : SocketEntity<Guid>, IMessage, IUpdateable
{
    #region SocketMessage

    private readonly List<SocketReaction> _reactions = [];
    private ImmutableArray<SocketUser> _userMentions = [];

    /// <summary>
    ///     Gets the author of this message.
    /// </summary>
    /// <returns>
    ///     A WebSocket-based user object.
    /// </returns>
    public SocketUser Author { get; }

    /// <summary>
    ///     Gets the source channel of the message.
    /// </summary>
    /// <returns>
    ///     A WebSocket-based message channel.
    /// </returns>
    public ISocketMessageChannel Channel { get; }

    /// <inheritdoc />
    public MessageSource Source { get; }

    /// <inheritdoc />
    public string Content { get; internal set; }

    /// <summary>
    ///     Gets the raw content of the message.
    /// </summary>
    /// <remarks>
    ///     This property is only available for messages that were received from the gateway.
    /// </remarks>
    public string RawContent { get; internal set; }

    /// <inheritdoc />
    public string CleanContent => MessageHelper.SanitizeMessage(this);

    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? EditedTimestamp { get; private set; }

    /// <inheritdoc cref="IMessage.IsPinned" />
    public virtual bool IsPinned { get; protected internal set; }

    /// <inheritdoc />
    public virtual bool MentionedEveryone => false;

    /// <inheritdoc />
    public virtual bool MentionedHere => false;

    /// <inheritdoc/>
    public MessageType Type { get; private set; }

    /// <summary>
    ///     Gets the attachment included in this message.
    /// </summary>
    public virtual IReadOnlyCollection<Attachment> Attachments { get; private set; }

    /// <summary>
    ///     Returns all cards included in this message.
    /// </summary>
    /// <returns>
    ///     Collection of card objects.
    /// </returns>
    public virtual IReadOnlyCollection<ICard> Cards => [];

    /// <summary>
    ///     Returns all embeds included in this message.
    /// </summary>
    /// <returns>
    ///     Collection of embed objects.
    /// </returns>
    public virtual IReadOnlyCollection<IEmbed> Embeds => [];

    /// <summary>
    ///     Gets a collection of the <see cref="SocketPokeAction"/>'s on the message.
    /// </summary>
    /// <returns>
    ///     Collection of poke action objects.
    /// </returns>
    public virtual IReadOnlyCollection<SocketPokeAction> Pokes => [];

    /// <summary>
    ///     Returns the roles mentioned in this message.
    /// </summary>
    /// <returns>
    ///     Collection of WebSocket-based roles.
    /// </returns>
    public virtual IReadOnlyCollection<SocketRole> MentionedRoles => [];

    /// <summary>
    ///     Returns the users mentioned in this message.
    /// </summary>
    /// <returns>
    ///     Collection of WebSocket-based users.
    /// </returns>
    public IReadOnlyCollection<SocketUser> MentionedUsers => _userMentions;

    /// <inheritdoc />
    public virtual IReadOnlyCollection<ITag> Tags => [];

    /// <inheritdoc />
    public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => _reactions
        .GroupBy(r => r.Emote)
        .ToDictionary(
            x => x.Key,
            x => new ReactionMetadata
            {
                ReactionCount = x.Count(),
                IsMe = x.Any(y => y.UserId == Kook.CurrentUser?.Id)
            });

    internal SocketMessage(KookSocketClient kook, Guid id, ISocketMessageChannel channel,
        SocketUser author, MessageSource source)
        : base(kook, id)
    {
        Channel = channel;
        Author = author;
        Source = source;
        Content = string.Empty;
        RawContent = string.Empty;
        Attachments = [];
    }

    internal static SocketMessage Create(KookSocketClient kook, ClientState state,
        SocketUser author, ISocketMessageChannel channel,
        GatewayEvent<GatewayGroupMessageExtraData> gatewayEvent) =>
        MessageHelper.GetSource(author) is MessageSource.System
            ? SocketSystemMessage.Create(kook, state, author, channel, gatewayEvent)
            : SocketUserMessage.Create(kook, state, author, channel, gatewayEvent);

    internal static SocketMessage Create(KookSocketClient kook, ClientState state,
        SocketUser author, ISocketMessageChannel channel,
        GatewayEvent<GatewayPersonMessageExtraData> gatewayEvent) =>
        MessageHelper.GetSource(author) is MessageSource.System
            ? SocketSystemMessage.Create(kook, state, author, channel, gatewayEvent)
            : SocketUserMessage.Create(kook, state, author, channel, gatewayEvent);

    internal virtual void Update(ClientState state, GatewayEvent<GatewayGroupMessageExtraData> gatewayEvent)
    {
        Type = gatewayEvent.Type;
        Timestamp = gatewayEvent.MessageTimestamp;
        Content = gatewayEvent.Content;

        if (gatewayEvent.ExtraData.MentionedUsers is { } users)
        {
            _userMentions = users.Select(x =>
                    Channel.GetUserAsync(x, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser
                    ?? SocketUnknownUser.Create(Kook, state, x))
                .ToImmutableArray();
        }
    }

    internal virtual void Update(ClientState state, GatewayEvent<GatewayPersonMessageExtraData> gatewayEvent)
    {
        Type = gatewayEvent.Type;
        Timestamp = gatewayEvent.MessageTimestamp;
        Content = gatewayEvent.Content;

        if (gatewayEvent.ExtraData.MentionedUsers is { } users)
        {
            _userMentions = users.Select(x =>
                    Channel.GetUserAsync(x, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser
                    ?? SocketUnknownUser.Create(Kook, state, x))
                .ToImmutableArray();
        }
    }

    internal static SocketMessage Create(KookSocketClient kook, ClientState state,
        SocketUser author, ISocketMessageChannel channel, API.Message model) =>
        MessageHelper.GetSource(author) is MessageSource.System
            ? SocketSystemMessage.Create(kook, state, author, channel, model)
            : SocketUserMessage.Create(kook, state, author, channel, model);

    internal static SocketMessage Create(KookSocketClient kook, ClientState state,
        SocketUser author, ISocketMessageChannel channel, API.DirectMessage model) =>
        MessageHelper.GetSource(author) is MessageSource.System
            ? SocketSystemMessage.Create(kook, state, author, channel, model)
            : SocketUserMessage.Create(kook, state, author, channel, model);

    internal virtual void Update(ClientState state, API.Message model)
    {
        Type = model.Type;
        Timestamp = model.CreateAt;
        EditedTimestamp = model.UpdateAt;
        Content = model.Content;

        if (model.MentionedUsers is { } users)
        {
            _userMentions = users.Select(x =>
                    Channel.GetUserAsync(x, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser
                    ?? SocketUnknownUser.Create(Kook, state, x))
                .ToImmutableArray();
        }
    }

    internal virtual void Update(ClientState state, API.DirectMessage model)
    {
        Type = model.Type;
        Timestamp = model.CreateAt;
        EditedTimestamp = model.UpdateAt;
        Content = model.Content;
    }

    internal virtual void Update(ClientState state, MessageUpdateEvent model)
    {
        EditedTimestamp = model.UpdatedAt;
        Content = model.Content;

        if (model.MentionedUsers is { } users)
        {
            _userMentions = users.Select(x =>
                    Channel.GetUserAsync(x, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser
                    ?? SocketUnknownUser.Create(Kook, state, x))
                .ToImmutableArray();
        }
    }

    internal virtual void Update(ClientState state, DirectMessageUpdateEvent model)
    {
        EditedTimestamp = model.UpdatedAt;
        Content = model.Content;
    }

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) =>
        MessageHelper.DeleteAsync(this, Kook, options);


    internal void AddReaction(SocketReaction reaction) => _reactions.Add(reaction);

    internal void RemoveReaction(SocketReaction reaction)
    {
        if (_reactions.Contains(reaction))
            _reactions.Remove(reaction);
    }

    internal void ClearReactions() => _reactions.Clear();
    internal void RemoveReactionsForEmote(IEmote emote) =>
        _reactions.RemoveAll(x => x.Emote.Equals(emote));

    /// <inheritdoc />
    public Task UpdateAsync(RequestOptions? options = null) =>
        SocketMessageHelper.UpdateAsync(this, Kook, options);

    /// <inheritdoc />
    public Task AddReactionAsync(IEmote emote, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.AddReactionAsync(this, emote, Kook, options),
            IDMChannel => MessageHelper.AddDirectMessageReactionAsync(this, emote, Kook, options),
            _ => throw new NotSupportedException("The operation is not supported for this message type.")
        };

    /// <inheritdoc />
    public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.RemoveReactionAsync(this, user.Id, emote, Kook, options),
            IDMChannel => MessageHelper.RemoveDirectMessageReactionAsync(this, user.Id, emote, Kook, options),
            _ => throw new NotSupportedException("The operation is not supported for this message type.")
        };

    /// <inheritdoc />
    public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.RemoveReactionAsync(this, userId, emote, Kook, options),
            IDMChannel => MessageHelper.RemoveDirectMessageReactionAsync(this, userId, emote, Kook, options),
            _ => throw new NotSupportedException("The operation is not supported for this message type.")
        };

    /// <inheritdoc />
    public Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emote, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.GetReactionUsersAsync(this, emote, Kook, options),
            IDMChannel => MessageHelper.GetDirectMessageReactionUsersAsync(this, emote, Kook, options),
            _ => throw new NotSupportedException("The operation is not supported for this message type.")
        };

    #endregion

    /// <summary>
    ///     Gets the content of the message.
    /// </summary>
    /// <returns>
    ///     Content of the message.
    /// </returns>
    public override string ToString() => Content;

    internal SocketMessage Clone() => (SocketMessage)MemberwiseClone();

    #region IMessage

    /// <inheritdoc />
    IUser IMessage.Author => Author;

    /// <inheritdoc />
    IMessageChannel IMessage.Channel => Channel;

    /// <inheritdoc />
    IReadOnlyCollection<uint> IMessage.MentionedRoleIds => [..MentionedRoles.Select(x => x.Id)];

    /// <inheritdoc />
    IReadOnlyCollection<ulong> IMessage.MentionedUserIds => [..MentionedUsers.Select(x => x.Id)];

    /// <inheritdoc />
    IReadOnlyCollection<IAttachment> IMessage.Attachments => Attachments;

    /// <inheritdoc />
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;

    /// <inheritdoc />
    IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;

    /// <inheritdoc />
    IReadOnlyCollection<IPokeAction> IMessage.Pokes => Pokes;

    /// <inheritdoc />
    bool? IMessage.IsPinned => IsPinned;

    #endregion
}
