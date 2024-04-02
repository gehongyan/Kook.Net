using Kook.API.Gateway;
using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;

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
    private ImmutableArray<Attachment> _attachments = ImmutableArray.Create<Attachment>();
    private ImmutableArray<ICard> _cards = ImmutableArray.Create<ICard>();
    private ImmutableArray<IEmbed>? _embeds;
    private ImmutableArray<SocketPokeAction> _pokes;
    private ImmutableArray<SocketRole> _roleMentions = ImmutableArray.Create<SocketRole>();
    private ImmutableArray<SocketGuildChannel> _channelMentions = ImmutableArray.Create<SocketGuildChannel>();
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
    public override bool? MentionedEveryone => _isMentioningEveryone;

    /// <inheritdoc />
    public override bool? MentionedHere => _isMentioningHere;

    /// <inheritdoc />
    public override IReadOnlyCollection<ITag> Tags => _tags;

    internal SocketUserMessage(KookSocketClient kook, Guid id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
        : base(kook, id, channel, author, source)
    {
    }

    internal static new SocketUserMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel,
        GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        SocketUserMessage entity = new(kook, gatewayEvent.MessageId, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model, gatewayEvent);
        return entity;
    }

    internal static new SocketUserMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel,
        GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        SocketUserMessage entity = new(kook, gatewayEvent.MessageId, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model, gatewayEvent);
        return entity;
    }

    internal static new SocketUserMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel,
        API.Message model)
    {
        SocketUserMessage entity = new(kook, model.Id, channel, author, SocketMessageHelper.GetSource(model));
        entity.Update(state, model);
        return entity;
    }

    internal static new SocketUserMessage Create(KookSocketClient kook, ClientState state, SocketUser author, ISocketMessageChannel channel,
        API.DirectMessage model)
    {
        SocketUserMessage entity = new(kook, model.Id, channel, author, SocketMessageHelper.GetSource(model, author));
        entity.Update(state, model);
        return entity;
    }

    internal override void Update(ClientState state, GatewayGroupMessageExtraData model, GatewayEvent gatewayEvent)
    {
        base.Update(state, model, gatewayEvent);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        _isMentioningEveryone = model.MentionedAll;
        _isMentioningHere = model.MentionedHere;
        _roleMentions = model.MentionedRoles?.Select(x => guild?.GetRole(x)).ToImmutableArray() ?? new ImmutableArray<SocketRole>();
        _channelMentions = model.MentionedChannels?.Select(x => guild?.GetChannel(x)).ToImmutableArray() ?? new ImmutableArray<SocketGuildChannel>();
        Content = gatewayEvent.Content;
        RawContent = model.KMarkdownInfo?.RawContent;
        if (Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(gatewayEvent.Content, Channel, guild, MentionedUsers, TagMode.PlainText);
        else if (Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(gatewayEvent.Content, Channel, guild, MentionedUsers, TagMode.KMarkdown);

        if (model.Quote is not null)
            _quote = Quote.Create(model.Quote.Id, model.Quote.QuotedMessageId, model.Quote.Type, model.Quote.Content,
                model.Quote.CreateAt, guild?.GetUser(model.Quote.Author.Id));

        if (model.Attachment is not null) _attachments = _attachments.Add(Attachment.Create(model.Attachment));

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(gatewayEvent.Content);
            _attachments = _attachments.AddRange(MessageHelper.ParseAttachments(_cards.OfType<Card>()));
        }

        _pokes = Type == MessageType.Poke && model.KMarkdownInfo?.Pokes is not null
            ? model.KMarkdownInfo.Pokes.Select(x => SocketPokeAction.Create(Kook, Author, MentionedUsers, x)).ToImmutableArray()
            : ImmutableArray<SocketPokeAction>.Empty;

        IsPinned = false;
        Guild = guild;
    }

    internal override void Update(ClientState state, GatewayPersonMessageExtraData model, GatewayEvent gatewayEvent)
    {
        base.Update(state, model, gatewayEvent);
        Content = gatewayEvent.Content;
        RawContent = model.KMarkdownInfo?.RawContent;
        if (model.Quote is not null)
            _quote = Quote.Create(model.Quote.Id,
                model.Quote.QuotedMessageId, model.Quote.Type, model.Quote.Content, model.Quote.CreateAt,
                state.GetOrAddUser(model.Quote.Author.Id, _ => SocketGlobalUser.Create(Kook, state, model.Quote.Author)));

        if (model.Attachment is not null) _attachments = _attachments.Add(Attachment.Create(model.Attachment));

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(gatewayEvent.Content);
            _attachments = _attachments.AddRange(MessageHelper.ParseAttachments(_cards.OfType<Card>()));
        }

        if (Type == MessageType.Poke && model.KMarkdownInfo?.Pokes is not null)
        {
            SocketUser recipient = (Channel as SocketDMChannel)?.Recipient;
            SocketUser target = recipient is null
                ? null
                : recipient.Id == Author.Id
                    ? Kook.CurrentUser
                    : recipient;
            _pokes = model.KMarkdownInfo.Pokes.Select(x => SocketPokeAction.Create(Kook, Author,
                new[] { target }, x)).ToImmutableArray();
        }
        else
            _pokes = ImmutableArray<SocketPokeAction>.Empty;
    }


    internal override void Update(ClientState state, API.Message model)
    {
        base.Update(state, model);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        _isMentioningEveryone = model.MentionedAll;
        _isMentioningHere = model.MentionedHere;
        _roleMentions = model.MentionedRoles?.Select(x => guild.GetRole(x)).ToImmutableArray() ?? new ImmutableArray<SocketRole>();
        _channelMentions = model.MentionInfo?.MentionedChannels?.Select(x => guild.GetChannel(x.Id)).ToImmutableArray()
            ?? new ImmutableArray<SocketGuildChannel>();
        Content = model.Content;
        if (Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText);
        else if (Type == MessageType.KMarkdown) _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown);

        if (model.Attachment is not null) _attachments = _attachments.Add(Attachment.Create(model.Attachment));

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(model.Content);
            _attachments = _attachments.AddRange(MessageHelper.ParseAttachments(_cards.OfType<Card>()));
        }

        _embeds = model.Embeds.Select(x => x.ToEntity()).ToImmutableArray();
        _pokes = Type == MessageType.Poke && model.MentionInfo?.Pokes is not null
            ? model.MentionInfo.Pokes.Select(x => SocketPokeAction.Create(Kook, Author,
                model.MentionedUsers.Select(state.GetUser), x)).ToImmutableArray()
            : ImmutableArray<SocketPokeAction>.Empty;

        Guild = guild;
    }

    internal override void Update(ClientState state, API.DirectMessage model)
    {
        base.Update(state, model);
        SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;
        Content = model.Content;
        if (Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.PlainText);
        else if (Type == MessageType.KMarkdown) _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown);

        if (model.Attachment is not null) _attachments = _attachments.Add(Attachment.Create(model.Attachment));

        if (Type == MessageType.Card)
        {
            _cards = MessageHelper.ParseCards(model.Content);
            _attachments = _attachments.AddRange(MessageHelper.ParseAttachments(_cards.OfType<Card>()));
        }

        _embeds = model.Embeds.Select(x => x.ToEntity()).ToImmutableArray();
        if (Type == MessageType.Poke && model.MentionInfo?.Pokes is not null)
        {
            SocketUser recipient = (Channel as SocketDMChannel)?.Recipient;
            SocketUser target = recipient is null
                ? null
                : recipient.Id == Author.Id
                    ? Kook.CurrentUser
                    : recipient;
            _pokes = model.MentionInfo.Pokes.Select(x => SocketPokeAction.Create(Kook, Author,
                new[] { target }, x)).ToImmutableArray();
        }
        else
            _pokes = ImmutableArray<SocketPokeAction>.Empty;

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
        else if (Type == MessageType.KMarkdown) _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown);

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
        else if (Type == MessageType.KMarkdown) _tags = MessageHelper.ParseTags(model.Content, Channel, Guild, MentionedUsers, TagMode.KMarkdown);

        _cards = Type == MessageType.Card
            ? MessageHelper.ParseCards(model.Content)
            : ImmutableArray.Create<ICard>();

        Guild = guild;
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Only the author of a message may modify the message.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="KookConfig.MaxMessageSize"/>.</exception>
    public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        => MessageHelper.ModifyAsync(this, Kook, func, options);

    /// <summary>
    ///     Transforms this message's text into a human-readable form by resolving its tags.
    /// </summary>
    /// <param name="startIndex">The zero-based index at which to begin the resolving for the specified value.</param>
    /// <param name="userHandling">Determines how the user tag should be handled.</param>
    /// <param name="channelHandling">Determines how the channel tag should be handled.</param>
    /// <param name="roleHandling">Determines how the role tag should be handled.</param>
    /// <param name="everyoneHandling">Determines how the @everyone tag should be handled.</param>
    /// <param name="emojiHandling">Determines how the emoji tag should be handled.</param>
    public string Resolve(int startIndex, TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
        TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Name, TagHandling emojiHandling = TagHandling.Name)
        => MentionUtils.Resolve(this, startIndex, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);

    /// <inheritdoc />
    public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
        TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Name, TagHandling emojiHandling = TagHandling.Name)
        => MentionUtils.Resolve(this, 0, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);

    private string DebuggerDisplay =>
        $"{Author}: {Content} ({Id}{(Attachments is not null && Attachments.Any() ? $", {Attachments.Count} Attachment{(Attachments.Count == 1 ? string.Empty : "s")}" : string.Empty)})";

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
