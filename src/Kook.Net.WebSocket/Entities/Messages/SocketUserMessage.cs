using Kook.API.Gateway;
using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based message sent by a user.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketUserMessage : SocketMessage, IUserMessage
{
    private bool _isMentioningEveryone;
    private bool _isMentioningHere;
    private ImmutableArray<Attachment> _attachments = [];
    private ImmutableArray<ICard> _cards = [];
    private ImmutableArray<IEmbed> _embeds = [];
    private ImmutableArray<SocketPokeAction> _pokes = [];
    private ImmutableArray<SocketRole> _roleMentions = [];
    private ImmutableArray<SocketGuildChannel> _channelMentions = [];
    private ImmutableArray<ITag> _tags = [];

    /// <inheritdoc />
    public IQuote? Quote { get; private set; }

    /// <summary>
    ///     Gets the <see cref="SocketGuild"/> that the message was sent from.
    /// </summary>
    /// <returns> The <see cref="SocketGuild"/> that the message was sent from. </returns>
    public SocketGuild? Guild { get; private set; }

    /// <inheritdoc />
    public override bool IsPinned { get; protected internal set; }

    /// <inheritdoc />
    public override IReadOnlyCollection<Attachment> Attachments => _attachments;

    /// <inheritdoc />
    public override IReadOnlyCollection<ICard> Cards => _cards;

    /// <inheritdoc />
    public override IReadOnlyCollection<IEmbed> Embeds => _embeds;

    /// <inheritdoc />
    public override IReadOnlyCollection<SocketPokeAction> Pokes => _pokes;

    /// <inheritdoc />
    public override IReadOnlyCollection<SocketRole> MentionedRoles => _roleMentions;

    /// <summary>
    ///     Gets a collection of the mentioned channels in the message.
    /// </summary>
    public IReadOnlyCollection<SocketGuildChannel> MentionedChannels => _channelMentions;

    /// <inheritdoc />
    public override bool MentionedEveryone => _isMentioningEveryone;

    /// <inheritdoc />
    public override bool MentionedHere => _isMentioningHere;

    /// <inheritdoc />
    public override IReadOnlyCollection<ITag> Tags => _tags;

    internal SocketUserMessage(KookSocketClient kook, Guid id,
        ISocketMessageChannel channel, SocketUser author, MessageSource source)
        : base(kook, id, channel, author, source)
    {
    }

    internal static new SocketUserMessage Create(KookSocketClient kook, ClientState state,
        SocketUser author, ISocketMessageChannel channel,
        GatewayEvent<GatewayGroupMessageExtraData> gatewayEvent)
    {
        MessageSource messageSource = SocketMessageHelper.GetSource(gatewayEvent.ExtraData);
        SocketUserMessage entity = new(kook, gatewayEvent.MessageId, channel, author, messageSource);
        entity.Update(state, gatewayEvent);
        return entity;
    }

    internal static new SocketUserMessage Create(KookSocketClient kook, ClientState state,
        SocketUser author, ISocketMessageChannel channel,
        GatewayEvent<GatewayPersonMessageExtraData> gatewayEvent)
    {
        MessageSource messageSource = SocketMessageHelper.GetSource(gatewayEvent.ExtraData);
        SocketUserMessage entity = new(kook, gatewayEvent.MessageId, channel, author, messageSource);
        entity.Update(state, gatewayEvent);
        return entity;
    }

    internal static new SocketUserMessage Create(KookSocketClient kook, ClientState state,
        SocketUser author, ISocketMessageChannel channel, API.Message model)
    {
        MessageSource messageSource = MessageHelper.GetSource(model);
        SocketUserMessage entity = new(kook, model.Id, channel, author, messageSource);
        entity.Update(state, model);
        return entity;
    }

    internal static new SocketUserMessage Create(KookSocketClient kook, ClientState state,
        SocketUser author, ISocketMessageChannel channel, API.DirectMessage model)
    {
        MessageSource messageSource = MessageHelper.GetSource(author);
        SocketUserMessage entity = new(kook, model.Id, channel, author, messageSource);
        entity.Update(state, model);
        return entity;
    }

    internal override void Update(ClientState state, GatewayEvent<GatewayGroupMessageExtraData> gatewayEvent)
    {
        base.Update(state, gatewayEvent);

        GatewayGroupMessageExtraData model = gatewayEvent.ExtraData;
        Content = gatewayEvent.Content;
        RawContent = model.KMarkdownInfo?.RawContent ?? gatewayEvent.Content;
        _isMentioningEveryone = model.MentionedAll;
        _isMentioningHere = model.MentionedHere;

        Guild = (Channel as SocketGuildChannel)?.Guild;
        if (Guild is not null)
        {
            if (model.MentionedRoles is { } roles)
                _roleMentions = [..roles.Select(x => Guild.GetRole(x) ?? new SocketRole(Guild, x))];
            if (model.MentionedChannels is { } channels)
                _channelMentions = [..channels.Select(x => Guild.GetChannel(x) ?? new SocketGuildChannel(Kook, x, Guild))];
        }

        _tags = model.Type switch
        {
            MessageType.Text => MessageHelper.ParseTags(gatewayEvent.Content, Channel, Guild, MentionedUsers, TagMode.PlainText),
            MessageType.KMarkdown => MessageHelper.ParseTags(gatewayEvent.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown),
            _ => _tags
        };

        if (model.Quote is { } quote)
        {
            IUser author = Guild?.GetUser(model.Quote.Author.Id)
                ?? Guild?.AddOrUpdateUser(model.Quote.Author)
                ?? state.GetOrAddUser(model.Quote.Author.Id, _ => SocketGlobalUser.Create(Kook, state, model.Quote.Author)) as IUser;
            Guid? quotedMessageId = quote.RongId ?? quote.QuotedMessageId;
            if (quotedMessageId == Guid.Empty)
                Quote = null;
            else if (quotedMessageId.HasValue)
                Quote = global::Kook.Quote.Create(quotedMessageId.Value, quote.Type, quote.Content, quote.CreateAt, author);
        }
        else
            Quote = null;

        if (model.Attachment is { } attachment)
            _attachments = [.._attachments, Attachment.Create(attachment)];

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(gatewayEvent.Content);
            _attachments = [.._attachments, ..MessageHelper.ParseAttachments(_cards.OfType<Card>())];
        }

        if (Type == MessageType.Poke && model.KMarkdownInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => SocketPokeAction.Create(Kook, Author, MentionedUsers, x))];

        IsPinned = false;
    }

    internal override void Update(ClientState state, GatewayEvent<GatewayPersonMessageExtraData> gatewayEvent)
    {
        base.Update(state, gatewayEvent);

        GatewayPersonMessageExtraData model = gatewayEvent.ExtraData;
        Content = gatewayEvent.Content;
        RawContent = model.KMarkdownInfo?.RawContent ?? gatewayEvent.Content;
        _attachments = [];

        if (model.Quote is { } quote)
        {
            IUser author = Guild?.GetUser(model.Quote.Author.Id)
                ?? Guild?.AddOrUpdateUser(model.Quote.Author)
                ?? state.GetOrAddUser(model.Quote.Author.Id, _ => SocketGlobalUser.Create(Kook, state, model.Quote.Author)) as IUser;
            Guid? quotedMessageId = quote.RongId ?? quote.QuotedMessageId;
            if (quotedMessageId == Guid.Empty)
                Quote = null;
            else if (quotedMessageId.HasValue)
                Quote = global::Kook.Quote.Create(quotedMessageId.Value, quote.Type, quote.Content, quote.CreateAt, author);
        }
        else
            Quote = null;

        if (model.Attachment is { } attachment)
            _attachments = [.._attachments, Attachment.Create(attachment)];

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(gatewayEvent.Content);
            _attachments = [.._attachments, ..MessageHelper.ParseAttachments(_cards.OfType<Card>())];
        }

        if (Type == MessageType.Poke && model.KMarkdownInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => SocketPokeAction.Create(Kook, Author, MentionedUsers, x))];
    }

    internal override void Update(ClientState state, API.Message model)
    {
        base.Update(state, model);

        Content = model.Content;
        _isMentioningEveryone = model.MentionedAll;
        _isMentioningHere = model.MentionedHere;
        _attachments = [];

        Guild = (Channel as SocketGuildChannel)?.Guild;
        if (Guild is not null)
        {
            _roleMentions = [..model.MentionedRoles.Select(x => Guild.GetRole(x) ?? new SocketRole(Guild, x))];
            if (model.MentionInfo?.MentionedChannels is { } channels)
                _channelMentions = [..channels.Select(x => Guild.GetChannel(x.Id) ?? new SocketGuildChannel(Kook, x.Id, Guild))];
        }

        _tags = model.Type switch
        {
            MessageType.Text => MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText),
            MessageType.KMarkdown => MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown),
            _ => _tags
        };

        if (model.Quote is { } quote)
        {
            IUser author = Guild?.GetUser(model.Quote.Author.Id)
                ?? Guild?.AddOrUpdateUser(model.Quote.Author)
                ?? state.GetOrAddUser(model.Quote.Author.Id, _ => SocketGlobalUser.Create(Kook, state, model.Quote.Author)) as IUser;
            Guid? quotedMessageId = quote.RongId ?? quote.QuotedMessageId;
            if (quotedMessageId == Guid.Empty)
                Quote = null;
            else if (quotedMessageId.HasValue)
                Quote = global::Kook.Quote.Create(quotedMessageId.Value, quote.Type, quote.Content, quote.CreateAt, author);
        }
        else
            Quote = null;

        if (model.Attachment is { } attachment)
            _attachments = [.._attachments, Attachment.Create(attachment)];

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(model.Content);
            _attachments = _attachments.AddRange(MessageHelper.ParseAttachments(_cards.OfType<Card>()));
        }

        _embeds = [..model.Embeds.Select(x => x.ToEntity())];

        if (Type == MessageType.Poke && model.MentionInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => SocketPokeAction.Create(Kook, Author, MentionedUsers, x))];
    }

    internal override void Update(ClientState state, API.DirectMessage model)
    {
        base.Update(state, model);

        Content = model.Content;
        _attachments = [];

        _tags = model.Type switch
        {
            MessageType.Text => MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText),
            MessageType.KMarkdown => MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown),
            _ => _tags
        };

        if (model.Quote is { } quote)
        {
            IUser author = Guild?.GetUser(model.Quote.Author.Id)
                ?? Guild?.AddOrUpdateUser(model.Quote.Author)
                ?? state.GetOrAddUser(model.Quote.Author.Id, _ => SocketGlobalUser.Create(Kook, state, model.Quote.Author)) as IUser;
            Guid? quotedMessageId = quote.RongId ?? quote.QuotedMessageId;
            if (quotedMessageId == Guid.Empty)
                Quote = null;
            else if (quotedMessageId.HasValue)
                Quote = global::Kook.Quote.Create(quotedMessageId.Value, quote.Type, quote.Content, quote.CreateAt, author);
        }
        else
            Quote = null;

        if (model.Attachment is { } attachment)
            _attachments = [.._attachments, Attachment.Create(attachment)];

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(model.Content);
            _attachments = _attachments.AddRange(MessageHelper.ParseAttachments(_cards.OfType<Card>()));
        }

        _embeds = [..model.Embeds.Select(x => x.ToEntity())];

        if (Type == MessageType.Poke && model.MentionInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => SocketPokeAction.Create(Kook, Author, MentionedUsers, x))];
    }

    internal override void Update(ClientState state, MessageUpdateEvent model)
    {
        base.Update(state, model);

        Content = model.Content;
        _isMentioningEveryone = model.MentionedAll;
        _isMentioningHere = model.MentionedHere;
        _attachments = [];

        Guild = (Channel as SocketGuildChannel)?.Guild;
        if (Guild is not null)
        {
            _roleMentions = [..model.MentionedRoles.Select(x => Guild.GetRole(x) ?? new SocketRole(Guild, x))];
            _channelMentions = [..model.MentionInfo.MentionedChannels.Select(x => Guild.GetChannel(x.Id) ?? new SocketGuildChannel(Kook, x.Id, Guild))];
        }

        _tags = Type switch
        {
            MessageType.Text => MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText),
            MessageType.KMarkdown => MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown),
            _ => _tags
        };

        if (model.Quote is { } quote)
        {
            IUser author = Guild?.GetUser(model.Quote.Author.Id)
                ?? Guild?.AddOrUpdateUser(model.Quote.Author)
                ?? state.GetOrAddUser(model.Quote.Author.Id, _ => SocketGlobalUser.Create(Kook, state, model.Quote.Author)) as IUser;
            Guid? quotedMessageId = quote.RongId ?? quote.QuotedMessageId;
            if (quotedMessageId == Guid.Empty)
                Quote = null;
            else if (quotedMessageId.HasValue)
                Quote = global::Kook.Quote.Create(quotedMessageId.Value, quote.Type, quote.Content, quote.CreateAt, author);
        }
        else
            Quote = null;

        if (model.Attachment is { } attachment)
            _attachments = [.._attachments, Attachment.Create(attachment)];

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(model.Content);
            _attachments = [.._attachments, ..MessageHelper.ParseAttachments(_cards.OfType<Card>())];
        }

        if (Type == MessageType.Poke && model.MentionInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => SocketPokeAction.Create(Kook, Author, MentionedUsers, x))];

        _embeds = [];
    }

    internal override void Update(ClientState state, EmbedsAppendEvent model)
    {
        _embeds = [..model.Embeds.Select(x => x.ToEntity())];
    }

    internal override void Update(ClientState state, DirectMessageUpdateEvent model)
    {
        base.Update(state, model);
        Content = model.Content;
        _attachments = [];

        _tags = Type switch
        {
            MessageType.Text => MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText),
            MessageType.KMarkdown => MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown),
            _ => _tags
        };

        if (model.Quote is { } quote)
        {
            IUser author = Guild?.GetUser(model.Quote.Author.Id)
                ?? Guild?.AddOrUpdateUser(model.Quote.Author)
                ?? state.GetOrAddUser(model.Quote.Author.Id, _ => SocketGlobalUser.Create(Kook, state, model.Quote.Author)) as IUser;
            Guid? quotedMessageId = quote.RongId ?? quote.QuotedMessageId;
            if (quotedMessageId == Guid.Empty)
                Quote = null;
            else if (quotedMessageId.HasValue)
                Quote = global::Kook.Quote.Create(quotedMessageId.Value, quote.Type, quote.Content, quote.CreateAt, author);
        }
        else
            Quote = null;

        if (model.Attachment is { } attachment)
            _attachments = [.._attachments, Attachment.Create(attachment)];

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(model.Content);
            _attachments = [.._attachments, ..MessageHelper.ParseAttachments(_cards.OfType<Card>())];
        }

        if (Type == MessageType.Poke && model.MentionInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => SocketPokeAction.Create(Kook, Author, MentionedUsers, x))];
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Only the author of a message may modify the message.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="KookConfig.MaxMessageSize"/>.</exception>
    public Task ModifyAsync(Action<MessageProperties> func, RequestOptions? options = null) =>
        MessageHelper.ModifyAsync(this, Kook, func, options);

    /// <summary>
    ///     Transforms this message's text into a human-readable form by resolving its tags.
    /// </summary>
    /// <param name="startIndex">The zero-based index at which to begin the resolving for the specified value.</param>
    /// <param name="userHandling">Determines how the user tag should be handled.</param>
    /// <param name="channelHandling">Determines how the channel tag should be handled.</param>
    /// <param name="roleHandling">Determines how the role tag should be handled.</param>
    /// <param name="everyoneHandling">Determines how the @everyone tag should be handled.</param>
    /// <param name="emojiHandling">Determines how the emoji tag should be handled.</param>
    public string Resolve(int startIndex, TagHandling userHandling = TagHandling.Name,
        TagHandling channelHandling = TagHandling.Name, TagHandling roleHandling = TagHandling.Name,
        TagHandling everyoneHandling = TagHandling.Name, TagHandling emojiHandling = TagHandling.Name) =>
        MentionUtils.Resolve(this, startIndex,
            userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);

    /// <inheritdoc />
    public string Resolve(TagHandling userHandling = TagHandling.Name,
        TagHandling channelHandling = TagHandling.Name, TagHandling roleHandling = TagHandling.Name,
        TagHandling everyoneHandling = TagHandling.Name, TagHandling emojiHandling = TagHandling.Name) =>
        MentionUtils.Resolve(this, 0,
            userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);

    private string DebuggerDisplay => $"{Author}: {Content} ({Id}{
        Attachments.Count switch
        {
            0 => string.Empty,
            1 => ", 1 Attachment",
            _ => $", {Attachments.Count} Attachments"
        }})";

    internal new SocketUserMessage Clone() => (SocketUserMessage)MemberwiseClone();

    #region IUserMessage

    /// <inheritdoc />
    bool? IMessage.IsPinned => IsPinned;

    /// <inheritdoc />
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;

    /// <inheritdoc />
    IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;

    #endregion
}
