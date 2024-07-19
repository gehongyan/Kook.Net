using Kook.API;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based message sent by a user.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestUserMessage : RestMessage, IUserMessage
{
    private bool _isMentioningEveryone;
    private bool _isMentioningHere;
    private ImmutableArray<Attachment> _attachments = [];
    private ImmutableArray<ICard> _cards = [];
    private ImmutableArray<IEmbed> _embeds = [];
    private ImmutableArray<RestPokeAction> _pokes = [];
    private ImmutableArray<uint> _roleMentionIds = [];
    private ImmutableArray<RestRole> _roleMentions = [];
    private ImmutableArray<RestGuildChannel> _channelMentions = [];
    private ImmutableArray<ITag> _tags = [];

    /// <inheritdoc />
    public IQuote? Quote { get; private set; }

    /// <inheritdoc />
    public override bool? IsPinned { get; internal set; }

    /// <inheritdoc />
    public override IReadOnlyCollection<Attachment> Attachments => _attachments;

    /// <inheritdoc />
    public override IReadOnlyCollection<ICard> Cards => _cards;

    /// <inheritdoc />
    public override IReadOnlyCollection<IEmbed> Embeds => _embeds;

    /// <inheritdoc />
    public override IReadOnlyCollection<RestPokeAction> Pokes => _pokes;

    /// <inheritdoc />
    public override bool MentionedEveryone => _isMentioningEveryone;

    /// <inheritdoc />
    public override bool MentionedHere => _isMentioningHere;

    /// <inheritdoc />
    public override IReadOnlyCollection<uint> MentionedRoleIds => _roleMentionIds;

    /// <summary>
    ///     Gets a collection of the mentioned roles in the message.
    /// </summary>
    public IReadOnlyCollection<RestRole> MentionedRoles => _roleMentions;

    /// <summary>
    ///     Gets a collection of the mentioned channels in the message.
    /// </summary>
    public IReadOnlyCollection<RestGuildChannel> MentionedChannels => _channelMentions;

    /// <inheritdoc />
    public override IReadOnlyCollection<ITag> Tags => _tags;

    internal RestUserMessage(BaseKookClient kook, Guid id, MessageType messageType,
        IMessageChannel channel, IUser author, MessageSource source)
        : base(kook, id, messageType, channel, author, source)
    {
    }

    internal static new RestUserMessage Create(BaseKookClient kook,
        IMessageChannel channel, IUser author, Message model)
    {
        MessageSource messageSource = MessageHelper.GetSource(model);
        RestUserMessage entity = new(kook, model.Id, model.Type, channel, author, messageSource);
        entity.Update(model);
        return entity;
    }

    internal static new RestUserMessage Create(BaseKookClient kook,
        IMessageChannel channel, IUser author, DirectMessage model)
    {
        MessageSource messageSource = MessageHelper.GetSource(author);
        RestUserMessage entity = new(kook, model.Id, model.Type, channel, author, messageSource);
        entity.Update(model);
        return entity;
    }

    internal override void Update(Message model)
    {
        base.Update(model);
        IGuild? guild = (Channel as IGuildChannel)?.Guild;
        _tags = model.Type switch
        {
            MessageType.Text => MessageHelper.ParseTags(model.Content, Channel, guild, MentionedUsers, TagMode.PlainText),
            MessageType.KMarkdown => MessageHelper.ParseTags(model.Content, Channel, guild, MentionedUsers, TagMode.KMarkdown),
            _ => _tags
        };

        _isMentioningEveryone = model.MentionedAll;
        _isMentioningHere = model.MentionedHere;
        _roleMentionIds = [..model.MentionedRoles];

        if (Channel is IGuildChannel guildChannel)
        {
            if (model.MentionInfo?.MentionedRoles is { } roles)
                _roleMentions = [..roles.Select(x => RestRole.Create(Kook, guildChannel.Guild, x))];
            if (model.MentionInfo?.MentionedChannels is { } channels)
                _channelMentions = [..channels.Select(x => RestGuildChannel.Create(Kook, guildChannel.Guild, x))];
        }

        if (model.Quote is { } quote)
        {
            IUser refMsgAuthor = MessageHelper.GetAuthor(Kook, guild, model.Quote.Author);
            Guid? quotedMessageId = quote.RongId ?? quote.QuotedMessageId;
            if (quotedMessageId == Guid.Empty)
                Quote = null;
            else if (quotedMessageId.HasValue)
                Quote = global::Kook.Quote.Create(quotedMessageId.Value, quote.Type, quote.Content, quote.CreateAt, refMsgAuthor);
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

        _embeds = [..model.Embeds.Select(x => x.ToEntity())];

        if (Type == MessageType.Poke && model.MentionInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => RestPokeAction.Create(Kook, Author, MentionedUsers, x))];
    }

    internal override void Update(DirectMessage model)
    {
        base.Update(model);
        _tags = model.Type switch
        {
            MessageType.Text => MessageHelper.ParseTags(model.Content, Channel, null, MentionedUsers, TagMode.PlainText),
            MessageType.KMarkdown => MessageHelper.ParseTags(model.Content, Channel, null, MentionedUsers, TagMode.KMarkdown),
            _ => _tags
        };

        if (model.Quote is { } quote)
        {
            IUser refMsgAuthor = MessageHelper.GetAuthor(Kook, null, model.Quote.Author);
            Guid? quotedMessageId = quote.RongId ?? quote.QuotedMessageId;
            if (quotedMessageId == Guid.Empty)
                Quote = null;
            else if (quotedMessageId.HasValue)
                Quote = global::Kook.Quote.Create(quotedMessageId.Value, quote.Type, quote.Content, quote.CreateAt, refMsgAuthor);
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

        _embeds = [..model.Embeds.Select(x => x.ToEntity())];

        if (Type == MessageType.Poke && model.MentionInfo is { Pokes: { } pokes })
            _pokes = [..pokes.Select(x => RestPokeAction.Create(Kook, Author, MentionedUsers, x))];
    }

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
        TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name) =>
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

    #region IUserMessage

    /// <inheritdoc />
    public async Task ModifyAsync(Action<MessageProperties> func, RequestOptions? options = null)
    {
        await MessageHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        MessageProperties properties = new() { Content = Content, Cards = [..Cards], Quote = Quote };
        func(properties);
        Content = properties.Content;
        _cards = properties.Cards?.ToImmutableArray() ?? ImmutableArray<ICard>.Empty;
        Quote = properties.Quote?.QuotedMessageId == Guid.Empty ? null : properties.Quote;
    }

    /// <inheritdoc />
    bool? IMessage.IsPinned => IsPinned;

    /// <inheritdoc />
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;

    /// <inheritdoc />
    IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;

    #endregion
}
