using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using KaiHeiLa.API;
using KaiHeiLa.API.Gateway;
using KaiHeiLa.Net.Converters;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based message sent by a user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketUserMessage : SocketMessage, IUserMessage
{
    private bool? _isMentioningEveryone;
    private bool? _isMentioningHere;
    private Quote _quote;
    private Attachment _attachment;
    private ImmutableArray<ICard> _cards = ImmutableArray.Create<ICard>();
    private ImmutableArray<SocketRole> _roleMentions = ImmutableArray.Create<SocketRole>();

    public Quote Quote => _quote;
    public SocketGuild Guild { get; private set; }
    
    /// <inheritdoc />
    public override Attachment Attachment => _attachment;
    /// <inheritdoc />  
    public override IReadOnlyCollection<ICard> Cards => _cards;
    /// <inheritdoc />
    public override IReadOnlyCollection<SocketRole> MentionedRoles => _roleMentions;
    /// <inheritdoc />
    public override bool? MentionedEveryone => _isMentioningEveryone;
    /// <inheritdoc />
    public override bool? MentionedHere => _isMentioningHere;
    
    internal SocketUserMessage(KaiHeiLaSocketClient discord, Guid id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
        : base(discord, id, channel, author, source)
    {
    }
    internal new static SocketUserMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        var entity = new SocketUserMessage(kaiHeiLa, gatewayEvent.MessageId, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model, gatewayEvent);
        return entity;
    }
    internal override void Update(ClientState state, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        base.Update(state, model, gatewayEvent);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        _isMentioningEveryone = model.MentionAll;
        _isMentioningHere = model.MentionHere;
        _roleMentions = model.MentionRoles?.Select(x => guild.GetRole(x)).ToImmutableArray()
            ?? new ImmutableArray<SocketRole>();
        Content = gatewayEvent.Content;
        if (model.Quote is not null)
            _quote = Quote.Create(model.Quote, guild.GetUser(model.Quote.Author.Id));

        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);

        if (model.Type == MessageType.Card)
        {
            string json = gatewayEvent.Content;
            JsonSerializerOptions serializerOptions = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters =
                {
                    new CardConverter(),
                    new ModuleConverter(),
                    new ElementConverter()
                }
            };
            CardBase[] cardBases = JsonSerializer.Deserialize<CardBase[]>(json, serializerOptions);
            
            var cards = ImmutableArray.CreateBuilder<ICard>(cardBases.Length);
            foreach (CardBase cardBase in cardBases)
                cards.Add(cardBase.ToEntity());

            _cards = cards.ToImmutable();
        }
        else
        {
            _cards = ImmutableArray.Create<ICard>();
        }
        
        Guild = guild;
    }
    
    internal new static SocketUserMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        var entity = new SocketUserMessage(kaiHeiLa, gatewayEvent.MessageId, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model, gatewayEvent);
        return entity;
    }
    internal override void Update(ClientState state, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        base.Update(state, model, gatewayEvent);
        Content = gatewayEvent.Content;
        if (model.Quote is not null)
        {
            _quote = Quote.Create(model.Quote, state.GetUser(model.Quote.Author.Id));
        }
        
        if (model.Attachment is not null)
        {
            _attachment = Attachment.Create(model.Attachment);
        }

        if (model.Type == MessageType.Card)
        {
            string json = gatewayEvent.Content;
            JsonSerializerOptions serializerOptions = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters =
                {
                    new CardConverter(),
                    new ModuleConverter(),
                    new ElementConverter()
                }
            };
            CardBase[] cardBases = JsonSerializer.Deserialize<CardBase[]>(json, serializerOptions);
            
            var cards = ImmutableArray.CreateBuilder<ICard>(cardBases.Length);
            foreach (CardBase cardBase in cardBases)
                cards.Add(cardBase.ToEntity());

            _cards = cards.ToImmutable();
        }
        else
        {
            _cards = ImmutableArray.Create<ICard>();
        }
    }
    
    private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachment is not null ? ", Attachment" : "")})";
    internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;

    #region IUserMessage

    IQuote IUserMessage.Quote => _quote;
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;

    #endregion
}