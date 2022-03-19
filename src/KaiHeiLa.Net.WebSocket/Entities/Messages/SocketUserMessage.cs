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
    private ImmutableArray<ITag> _tags = ImmutableArray.Create<ITag>();

    public Quote Quote => _quote;
    public SocketGuild Guild { get; private set; }
    
    public new bool? IsPinned { get; internal set; }

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
    /// <inheritdoc />
    public override IReadOnlyCollection<ITag> Tags => _tags;
    
    internal SocketUserMessage(KaiHeiLaSocketClient kaiHeiLa, Guid id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
        : base(kaiHeiLa, id, channel, author, source)
    {
    }
    internal new static SocketUserMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        var entity = new SocketUserMessage(kaiHeiLa, gatewayEvent.MessageId, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model, gatewayEvent);
        return entity;
    }
    internal new static SocketUserMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, API.Message model)
    {
        var entity = new SocketUserMessage(kaiHeiLa, model.Id, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model);
        return entity;
    }
    internal new static SocketUserMessage Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, SocketUser author, ISocketMessageChannel channel, API.DirectMessage model)
    {
        var entity = new SocketUserMessage(kaiHeiLa, model.Id, channel, author, SocketMessageHelper.GetSource(model, author));
        entity.Update(state, model);
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
        if (model.Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(gatewayEvent.Content, Channel, guild, MentionedUsers, TagMode.PlainText);
        else if (model.Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(gatewayEvent.Content, Channel, guild, MentionedUsers, TagMode.KMarkdown);
        if (model.Quote is not null)
            _quote = Quote.Create(model.Quote, guild.GetUser(model.Quote.Author.Id));

        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);

        _cards = model.Type == MessageType.Card 
            ? MessageHelper.ParseCards(gatewayEvent.Content) 
            : ImmutableArray.Create<ICard>();
        
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

        _cards = model.Type == MessageType.Card 
            ? MessageHelper.ParseCards(gatewayEvent.Content) 
            : ImmutableArray.Create<ICard>();
    }

    internal override void Update(ClientState state, API.Message model)
    {
        base.Update(state, model);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        _isMentioningEveryone = model.MentionAll;
        _isMentioningHere = model.MentionHere;
        _roleMentions = model.MentionRoles?.Select(x => Guild.GetRole(x)).ToImmutableArray()
                        ?? new ImmutableArray<SocketRole>();
        Content = model.Content;
        if (Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText);
        else if (Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown);
        
        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);
        
        _cards = Type == MessageType.Card 
            ? MessageHelper.ParseCards(model.Content) 
            : ImmutableArray.Create<ICard>();
        
        Guild = guild;
    }
    internal override void Update(ClientState state, API.DirectMessage model)
    {
        base.Update(state, model);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        Content = model.Content;
        if (Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText);
        else if (Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown);
        
        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);
        
        _cards = Type == MessageType.Card 
            ? MessageHelper.ParseCards(model.Content) 
            : ImmutableArray.Create<ICard>();
        
        Guild = guild;
    }
    internal override void Update(ClientState state, MessageUpdateEvent model)
    {
        base.Update(state, model);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        _isMentioningEveryone = model.MentionAll;
        _isMentioningHere = model.MentionHere;
        _roleMentions = model.MentionRoles?.Select(x => Guild.GetRole(x)).ToImmutableArray()
                        ?? new ImmutableArray<SocketRole>();
        Content = model.Content;
        if (Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText);
        else if (Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown);
        
        _cards = Type == MessageType.Card 
            ? MessageHelper.ParseCards(model.Content) 
            : ImmutableArray.Create<ICard>();
        
        Guild = guild;
    }
    internal override void Update(ClientState state, DirectMessageUpdateEvent model)
    {
        base.Update(state, model);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        Content = model.Content;
        if (Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText);
        else if (Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown);
        
        _cards = Type == MessageType.Card 
            ? MessageHelper.ParseCards(model.Content) 
            : ImmutableArray.Create<ICard>();
        
        Guild = guild;
    }
    
    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Only the author of a message may modify the message.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="KaiHeiLaConfig.MaxMessageSize"/>.</exception>
    public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        => MessageHelper.ModifyAsync(this, KaiHeiLa, func, options);
    
    private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachment is not null ? ", Attachment" : "")})";
    internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;

    #region IUserMessage

    bool? IMessage.IsPinned => IsPinned;
    IQuote IUserMessage.Quote => _quote;
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;

    #endregion
}