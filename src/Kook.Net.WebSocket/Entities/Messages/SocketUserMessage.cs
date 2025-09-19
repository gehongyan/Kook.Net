using Kook.API.Gateway;
using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的用户消息。
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
    ///     获取此消息所属的服务器。
    /// </summary>
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
    ///     获取此消息中提及的所有频道。
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
            _attachments = [.._attachments, ..MessageHelper.ParseAttachments(_cards)];
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
            _attachments = [.._attachments, ..MessageHelper.ParseAttachments(_cards)];
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
            _attachments = _attachments.AddRange(MessageHelper.ParseAttachments(_cards));
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
            _attachments = _attachments.AddRange(MessageHelper.ParseAttachments(_cards));
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
            _attachments = [.._attachments, ..MessageHelper.ParseAttachments(_cards)];
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
            _attachments = [.._attachments, ..MessageHelper.ParseAttachments(_cards)];
        }

        if (Type == MessageType.Poke && model.MentionInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => SocketPokeAction.Create(Kook, Author, MentionedUsers, x))];
    }

    /// <inheritdoc />
    public Task ModifyAsync(Action<MessageProperties> func, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.ModifyAsync<object>(this, Kook, func, options),
            IDMChannel => MessageHelper.ModifyDirectAsync<object>(this, Kook, func, options),
            _ => Task.FromException<NotSupportedException>(new NotSupportedException("Unsupported channel type."))
        };

    /// <inheritdoc />
    public Task ModifyAsync<T>(Action<MessageProperties<T>> func, RequestOptions? options = null) =>
        Channel switch
        {
            ITextChannel => MessageHelper.ModifyAsync(this, Kook, func, options),
            IDMChannel => MessageHelper.ModifyDirectAsync(this, Kook, func, options),
            _ => Task.FromException<NotSupportedException>(new NotSupportedException("Unsupported channel type."))
        };

    /// <inheritdoc />
    public async Task PinAsync(RequestOptions? options = null) =>
        await MessageHelper.PinAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task UnpinAsync(RequestOptions? options = null) =>
        await MessageHelper.UnpinAsync(this, Kook, options).ConfigureAwait(false);

    /// <summary>
    ///     转换消息文本中的提及与表情符号为可读形式。
    /// </summary>
    /// <param name="startIndex"> 指定要开始解析的位置。 </param>
    /// <param name="userHandling"> 指定用户提及标签的处理方式。 </param>
    /// <param name="channelHandling"> 指定频道提及标签的处理方式。 </param>
    /// <param name="roleHandling"> 指定角色提及标签的处理方式。 </param>
    /// <param name="everyoneHandling"> 指定全体成员与在线成员提及标签的处理方式。 </param>
    /// <param name="emojiHandling"> 指定表情符号标签的处理方式。 </param>
    /// <returns> 转换后的消息文本。 </returns>
    /// <remarks>
    ///     此方法推荐适用于消息类型为 <see cref="Kook.MessageType.Text"/> 或
    ///     <see cref="Kook.MessageType.KMarkdown"/>，对于 <see cref="Kook.MessageType.Card"/> 类型的消息，参见
    ///     <see cref="Kook.MessageExtensions.TryExtractCardContent(Kook.IUserMessage,out System.String)"/>。
    /// </remarks>
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
