using System.Collections.Immutable;
using KaiHeiLa.API;
using KaiHeiLa.API.Gateway;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based message.
/// </summary>
public abstract class SocketMessage : SocketEntity<Guid>, IMessage, IReloadable
{
    #region SocketMessage

    private readonly List<SocketReaction> _reactions = new List<SocketReaction>();
    private ImmutableArray<SocketUser> _userMentions = ImmutableArray.Create<SocketUser>();
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
    /// <inheritdoc />
    public virtual bool? IsPinned => null;
    /// <inheritdoc />
    public virtual bool? MentionedEveryone => false;
    /// <inheritdoc />
    public virtual bool? MentionedHere => false;

    /// <inheritdoc/>
    public MessageType Type { get; private set; }

    /// <summary>
    ///     Gets the attachment included in this message.
    /// </summary>
    public virtual Attachment Attachment { get; private set; }

    /// <summary>
    ///     Returns all cards included in this message.
    /// </summary>
    /// <returns>
    ///     Collection of card objects.
    /// </returns>
    public virtual IReadOnlyCollection<ICard> Cards => ImmutableArray.Create<ICard>();
    /// <summary>
    ///     Returns all embeds included in this message.
    /// </summary>
    /// <returns>
    ///     Collection of embed objects.
    /// </returns>
    public virtual IReadOnlyCollection<IEmbed> Embeds => null;
    
    /// <summary>
    ///     Returns the roles mentioned in this message.
    /// </summary>
    /// <returns>
    ///     Collection of WebSocket-based roles.
    /// </returns>
    public virtual IReadOnlyCollection<SocketRole> MentionedRoles => ImmutableArray.Create<SocketRole>();
    /// <summary>
    ///     Returns the users mentioned in this message.
    /// </summary>
    /// <returns>
    ///     Collection of WebSocket-based users.
    /// </returns>
    public IReadOnlyCollection<SocketUser> MentionedUsers => _userMentions; 
    /// <inheritdoc />
    public virtual IReadOnlyCollection<ITag> Tags => ImmutableArray.Create<ITag>();
    /// <inheritdoc />
    public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => _reactions.GroupBy(r => r.Emote).ToDictionary(x => x.Key, x => new ReactionMetadata { ReactionCount = x.Count(), IsMe = x.Any(y => y.UserId == KaiHeiLa.CurrentUser.Id) });

    internal SocketMessage(KaiHeiLaSocketClient kaiHeiLa, Guid id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
        : base(kaiHeiLa, id)
    {
        Channel = channel;
        Author = author;
        Source = source;
    }
    internal static SocketMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        if (model.Type == MessageType.System)
            return SocketSystemMessage.Create(kaiHeiLa, state, author, channel, model, gatewayEvent);
        else
            return SocketUserMessage.Create(kaiHeiLa, state, author, channel, model, gatewayEvent);
    }
    internal virtual void Update(ClientState state, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        Type = model.Type;
        Timestamp = gatewayEvent.MessageTimestamp;
        Content = gatewayEvent.Content;
        
        if (model.Mention is not null)
        {
            var ids = model.Mention;
            if (ids.Length > 0)
            {
                var newMentions = ImmutableArray.CreateBuilder<SocketUser>(ids.Length);
                for (int i = 0; i < ids.Length; i++)
                {
                    var id = ids[i];
                    var user = Channel.GetUserAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser;
                    newMentions.Add(user ?? SocketUnknownUser.Create(KaiHeiLa, state, id));
                }
                _userMentions = newMentions.ToImmutable();
            }
        }
    }
    internal static SocketMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        if (model.Type == MessageType.System)
            return SocketSystemMessage.Create(kaiHeiLa, state, author, channel, model, gatewayEvent);
        else
            return SocketUserMessage.Create(kaiHeiLa, state, author, channel, model, gatewayEvent);
    }
    internal virtual void Update(ClientState state, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        Type = model.Type;
        Timestamp = gatewayEvent.MessageTimestamp;
        Content = gatewayEvent.Content;
    }
    internal static SocketMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, API.Message model)
    {
        if (model is null)
            return null;
        if (model.Type == MessageType.System)
            return SocketSystemMessage.Create(kaiHeiLa, state, author, channel, model);
        else
            return SocketUserMessage.Create(kaiHeiLa, state, author, channel, model);
    }
    internal static SocketMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, API.DirectMessage model)
    {
        if (model is null)
            return null;
        if (model.Type == MessageType.System)
            return SocketSystemMessage.Create(kaiHeiLa, state, author, channel, model);
        else
            return SocketUserMessage.Create(kaiHeiLa, state, author, channel, model);
    }
    internal virtual void Update(ClientState state, API.Message model)
    {
        Type = model.Type;
        Timestamp = model.CreateAt;
        EditedTimestamp = model.UpdateAt;
        Content = model.Content;
        
        if (model.Mention is not null)
        {
            var ids = model.Mention;
            if (ids.Length > 0)
            {
                var newMentions = ImmutableArray.CreateBuilder<SocketUser>(ids.Length);
                for (int i = 0; i < ids.Length; i++)
                {
                    var id = ids[i];
                    var user = Channel.GetUserAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser;
                    newMentions.Add(user ?? SocketUnknownUser.Create(KaiHeiLa, state, id));
                }
                _userMentions = newMentions.ToImmutable();
            }
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
        
        if (model.Mention is not null)
        {
            var ids = model.Mention;
            if (ids.Length > 0)
            {
                var newMentions = ImmutableArray.CreateBuilder<SocketUser>(ids.Length);
                for (int i = 0; i < ids.Length; i++)
                {
                    var id = ids[i];
                    var user = Channel.GetUserAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser;
                    newMentions.Add(user ?? SocketUnknownUser.Create(KaiHeiLa, state, id));
                }
                _userMentions = newMentions.ToImmutable();
            }
        }
    }
    internal virtual void Update(ClientState state, DirectMessageUpdateEvent model)
    {
        EditedTimestamp = model.UpdatedAt;
        Content = model.Content;
    }
    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions options = null)
        => MessageHelper.DeleteAsync(this, KaiHeiLa, options);
    
    
    internal void AddReaction(SocketReaction reaction)
    {
        _reactions.Add(reaction);
    }
    internal void RemoveReaction(SocketReaction reaction)
    {
        if (_reactions.Contains(reaction))
            _reactions.Remove(reaction);
    }
    internal void ClearReactions()
    {
        _reactions.Clear();
    }
    internal void RemoveReactionsForEmote(IEmote emote)
    {
        _reactions.RemoveAll(x => x.Emote.Equals(emote));
    }

    /// <inheritdoc />
    public Task ReloadAsync(RequestOptions options = null)
        => SocketMessageHelper.ReloadAsync(this, KaiHeiLa, options);
    
    /// <inheritdoc />
    public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
    {
        return Channel switch
        {
            ITextChannel => MessageHelper.AddReactionAsync(this, emote, KaiHeiLa, options),
            IDMChannel => MessageHelper.AddDirectMessageReactionAsync(this, emote, KaiHeiLa, options),
            _ => Task.CompletedTask
        };
    }

    /// <inheritdoc />
    public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
    {
        return Channel switch
        {
            ITextChannel => MessageHelper.RemoveReactionAsync(this, user.Id, emote, KaiHeiLa, options),
            IDMChannel => MessageHelper.RemoveDirectMessageReactionAsync(this, user.Id, emote, KaiHeiLa, options),
            _ => Task.CompletedTask
        };
    }

    /// <inheritdoc />
    public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null)
    {
        return Channel switch
        {
            ITextChannel => MessageHelper.RemoveReactionAsync(this, userId, emote, KaiHeiLa, options),
            IDMChannel => MessageHelper.RemoveDirectMessageReactionAsync(this, userId, emote, KaiHeiLa, options),
            _ => Task.CompletedTask
        };
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emote, RequestOptions options = null)
    {
        return Channel switch
        {
            ITextChannel => MessageHelper.GetReactionUsersAsync(this, emote, KaiHeiLa, options),
            IDMChannel => MessageHelper.GetDirectMessageReactionUsersAsync(this, emote, KaiHeiLa, options),
            _ => Task.FromResult<IReadOnlyCollection<IUser>>(null)
        };
    }

    #endregion

    /// <summary>
    ///     Gets the content of the message.
    /// </summary>
    /// <returns>
    ///     Content of the message.
    /// </returns>
    public override string ToString() => Content;
    internal SocketMessage Clone() => MemberwiseClone() as SocketMessage;
    
    #region IMessage

    /// <inheritdoc />
    IUser IMessage.Author => Author;
    /// <inheritdoc />
    IMessageChannel IMessage.Channel => Channel;
    /// <inheritdoc />
    IReadOnlyCollection<uint> IMessage.MentionedRoleIds => MentionedRoles.Select(x => x.Id).ToImmutableArray();
    /// <inheritdoc />
    IReadOnlyCollection<ulong> IMessage.MentionedUserIds => MentionedUsers.Select(x => x.Id).ToImmutableArray();
    /// <inheritdoc />
    IAttachment IMessage.Attachment => Attachment;
    /// <inheritdoc />
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;
    /// <inheritdoc />
    IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;
    #endregion
}