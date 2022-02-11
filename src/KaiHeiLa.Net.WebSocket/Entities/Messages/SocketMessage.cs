using System.Collections.Immutable;
using KaiHeiLa.API.Gateway;
using Model = KaiHeiLa.API.Gateway.GatewayMessageExtraData;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based message.
/// </summary>
public abstract class SocketMessage : SocketEntity<Guid>, IMessage
{
    #region SocketMessage

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
    public ChannelType ChannelType { get; }
    /// <inheritdoc />
    public MessageSource Source { get; }
    /// <inheritdoc />
    public string Content { get; internal set; }

    // TODO: Sanitize
    // public string CleanContent => MessageHelper.SanitizeMessage(this);
    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; private set; }
    /// <inheritdoc />
    public virtual bool MentionedEveryone => false;
    /// <inheritdoc />
    public virtual bool MentionedHere => false;

    /// <inheritdoc/>
    public MessageType Type { get; private set; }

    public virtual Attachment Attachment { get; private set; }

    public virtual IReadOnlyCollection<ICard> Cards => ImmutableArray.Create<ICard>();
    
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
    
    internal SocketMessage(KaiHeiLaSocketClient kaiHeiLa, Guid id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
        : base(kaiHeiLa, id)
    {
        Channel = channel;
        Author = author;
        Source = source;
    }
    internal static SocketMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model, GatewayEvent gatewayEvent)
    {
        if (model.Type == MessageType.System)
            return SocketSystemMessage.Create(kaiHeiLa, state, author, channel, model, gatewayEvent);
        else
            return SocketUserMessage.Create(kaiHeiLa, state, author, channel, model, gatewayEvent);
    }
    internal virtual void Update(ClientState state, Model model, GatewayEvent gatewayEvent)
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
    #endregion
}