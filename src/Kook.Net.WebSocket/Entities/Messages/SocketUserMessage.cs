using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API;
using Kook.API.Gateway;
using Kook.Net.Converters;
using Kook.Rest;

namespace Kook.WebSocket;

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
    private ImmutableArray<IEmbed>? _embeds;
    private ImmutableArray<SocketRole> _roleMentions = ImmutableArray.Create<SocketRole>();
    private ImmutableArray<ITag> _tags = ImmutableArray.Create<ITag>();

    /// <inheritdoc cref="IUserMessage.Quote"/>
    public Quote Quote => _quote;
    /// <summary>
    ///     Gets the <see cref="SocketGuild"/> that the message was sent from.
    /// </summary>
    /// <returns>
    ///     The <see cref="SocketGuild"/> that the message was sent from.
    /// </returns>
    public SocketGuild Guild { get; private set; }
    /// <inheritdoc cref="IMessage.IsPinned"/>
    public new bool? IsPinned { get; internal set; }

    /// <inheritdoc />
    public override Attachment Attachment => _attachment;
    /// <inheritdoc />  
    public override IReadOnlyCollection<ICard> Cards => _cards;
    /// <inheritdoc />  
    public override IReadOnlyCollection<IEmbed> Embeds => _embeds;
    /// <inheritdoc />
    public override IReadOnlyCollection<SocketRole> MentionedRoles => _roleMentions;
    /// <inheritdoc />
    public override bool? MentionedEveryone => _isMentioningEveryone;
    /// <inheritdoc />
    public override bool? MentionedHere => _isMentioningHere;
    /// <inheritdoc />
    public override IReadOnlyCollection<ITag> Tags => _tags;
    
    internal SocketUserMessage(KookSocketClient kook, Guid id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
        : base(kook, id, channel, author, source)
    {
    }
    internal new static SocketUserMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        var entity = new SocketUserMessage(kook, gatewayEvent.MessageId, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model, gatewayEvent);
        return entity;
    }
    internal new static SocketUserMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        var entity = new SocketUserMessage(kook, gatewayEvent.MessageId, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model, gatewayEvent);
        return entity;
    }
    internal new static SocketUserMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel, API.Message model)
    {
        var entity = new SocketUserMessage(kook, model.Id, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model);
        return entity;
    }
    internal new static SocketUserMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel, API.DirectMessage model)
    {
        var entity = new SocketUserMessage(kook, model.Id, channel, author, SocketMessageHelper.GetSource(model, author));
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
        RawContent = model.KMarkdownInfo?.RawContent;
        if (model.Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(gatewayEvent.Content, Channel, guild, MentionedUsers, TagMode.PlainText);
        else if (model.Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(gatewayEvent.Content, Channel, guild, MentionedUsers, TagMode.KMarkdown);
        if (model.Quote is not null)
            _quote = Quote.Create(model.Quote.Id, model.Quote.QuotedMessageId, model.Quote.Type, model.Quote.Content, 
                model.Quote.CreateAt, guild.GetUser(model.Quote.Author.Id));

        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);

        _cards = model.Type == MessageType.Card 
            ? MessageHelper.ParseCards(gatewayEvent.Content) 
            : ImmutableArray.Create<ICard>();
        
        Guild = guild;
    }
    
    internal override void Update(ClientState state, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        base.Update(state, model, gatewayEvent);
        Content = gatewayEvent.Content;
        RawContent = model.KMarkdownInfo?.RawContent;
        if (model.Quote is not null)
            _quote = Quote.Create(model.Quote.Id, model.Quote.QuotedMessageId, model.Quote.Type, model.Quote.Content,
                model.Quote.CreateAt, state.GetUser(model.Quote.Author.Id));

        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);

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
        _embeds = model.Embeds.Select(x => x.ToEntity()).ToImmutableArray();
        
        
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
        
        _embeds = model.Embeds.Select(x => x.ToEntity()).ToImmutableArray();
        
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
        
        // TODO: Investigate embed changes
        
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
        
        // TODO: Investigate embed changes
        
        Guild = guild;
    }
    
    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Only the author of a message may modify the message.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="KookConfig.MaxMessageSize"/>.</exception>
    public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        => MessageHelper.ModifyAsync(this, Kook, func, options);
    
    /// <param name="startIndex">The zero-based index at which to begin the resolving for the specified value.</param>
    /// <inheritdoc cref="IUserMessage.Resolve(TagHandling,TagHandling,TagHandling,TagHandling,TagHandling)"/>
    public string Resolve(int startIndex, TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
        TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
        => MentionUtils.Resolve(this, startIndex, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);
    /// <inheritdoc />
    public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
        TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
        => MentionUtils.Resolve(this, 0, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);
    
    private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachment is not null ? ", Attachment" : "")})";
    internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;

    #region IUserMessage

    /// <inheritdoc />
    bool? IMessage.IsPinned => IsPinned;
    /// <inheritdoc />
    IQuote IUserMessage.Quote => _quote;
    /// <inheritdoc />
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;
    /// <inheritdoc />
    IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;

    #endregion
}