using System.Collections.Immutable;
using System.Diagnostics;
using KaiHeiLa.API.Gateway;
using KaiHeiLa.Rest;
using Model = KaiHeiLa.API.Gateway.GatewayMessageExtraData;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based message sent by a user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketUserMessage : SocketMessage, IUserMessage
{
    private bool _isMentioningEveryone;
    private bool _isMentioningHere;
    private Quote _quote;
    private Attachment _attachment;
    private ImmutableArray<Card> _cards = ImmutableArray.Create<Card>();
    private ImmutableArray<SocketRole> _roleMentions = ImmutableArray.Create<SocketRole>();

    public Quote Quote => _quote;
    public SocketGuild Guild { get; private set; }
    
    /// <inheritdoc />
    public override Attachment Attachment => _attachment;
    /// <inheritdoc />  
    public override IReadOnlyCollection<Card> Cards => _cards;
    /// <inheritdoc />
    public override IReadOnlyCollection<SocketRole> MentionedRoles => _roleMentions;
    /// <inheritdoc />
    public override bool MentionedEveryone => _isMentioningEveryone;
    /// <inheritdoc />
    public override bool MentionedHere => _isMentioningHere;
    
    internal SocketUserMessage(KaiHeiLaSocketClient discord, Guid id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
        : base(discord, id, channel, author, source)
    {
    }
    internal new static SocketUserMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model, GatewayEvent gatewayEvent)
    {
        var entity = new SocketUserMessage(kaiHeiLa, gatewayEvent.MessageId, channel, author, MessageHelper.GetSource(model));
        entity.Update(state, model, gatewayEvent);
        return entity;
    }
    internal override void Update(ClientState state, Model model, GatewayEvent gatewayEvent)
    {
        base.Update(state, model, gatewayEvent);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        _isMentioningEveryone = model.MentionAll;
        _isMentioningHere = model.MentionHere;
        _roleMentions = model.MentionRoles.Select(x => guild.GetRole(x)).ToImmutableArray();
        Content = gatewayEvent.Content;
        if (model.Quote is not null)
        {
            _quote = Quote.Create(model.Quote, guild.GetUser(model.Quote.Author.Id));
        }

        if (model.Attachment is not null)
        {
            _attachment = Attachment.Create(model.Attachment);
        }

        if (model.Type == MessageType.Card)
        {
            // TODO 反序列化Cards
        }
        else
        {
            _cards = ImmutableArray.Create<Card>();
        }
        
        Guild = guild;
    }
    
    private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachment is not null ? ", Attachment" : "")})";
    internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;

    #region IUserMessage

    IGuild IUserMessage.Guild => Guild;
    IQuote IUserMessage.Quote => _quote;

    #endregion
}