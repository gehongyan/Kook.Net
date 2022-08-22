using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using Kook.API;
using Kook.Net.Converters;
using Model = Kook.API.Message;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based message sent by a user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestUserMessage : RestMessage, IUserMessage
{
    private bool? _isMentioningEveryone;
    private bool? _isMentioningHere;
    private Quote _quote;
    private Attachment _attachment;
    private ImmutableArray<ICard> _cards = ImmutableArray.Create<ICard>();
    private ImmutableArray<IEmbed>? _embeds;
    private ImmutableArray<uint> _roleMentionIds = ImmutableArray.Create<uint>();
    private ImmutableArray<ITag> _tags = ImmutableArray.Create<ITag>();
    
    /// <inheritdoc cref="IUserMessage.Quote"/>
    public Quote Quote => _quote;
    
    /// <inheritdoc cref="IMessage.IsPinned"/>
    public new bool? IsPinned { get; internal set; }
    /// <inheritdoc />
    public override Attachment Attachment => _attachment;
    /// <inheritdoc />  
    public override IReadOnlyCollection<ICard> Cards => _cards;
    /// <inheritdoc />  
    public override IReadOnlyCollection<IEmbed> Embeds => _embeds;
    /// <inheritdoc />
    public override bool? MentionedEveryone => _isMentioningEveryone;
    /// <inheritdoc />
    public override bool? MentionedHere => _isMentioningHere;
    /// <inheritdoc />
    public override IReadOnlyCollection<uint> MentionedRoleIds => _roleMentionIds;
    /// <inheritdoc />
    public override IReadOnlyCollection<ITag> Tags => _tags;
    
    internal RestUserMessage(BaseKookClient kook, Guid id, MessageType messageType, IMessageChannel channel, IUser author, MessageSource source)
        : base(kook, id, messageType, channel, author, source)
    {
    }
    internal new static RestUserMessage Create(BaseKookClient kook, IMessageChannel channel, IUser author, Model model)
    {
        var entity = new RestUserMessage(kook, model.Id, model.Type, channel, author, MessageHelper.GetSource(model));
        entity.Update(model);
        return entity;
    }
    internal new static RestUserMessage Create(BaseKookClient kook, IMessageChannel channel, IUser author, DirectMessage model)
    {
        var entity = new RestUserMessage(kook, model.Id, model.Type, channel, author, MessageHelper.GetSource(model, author));
        entity.Update(model);
        return entity;
    }

    internal override void Update(Model model)
    {
        base.Update(model);
        var guildId = (Channel as IGuildChannel)?.GuildId;
        var guild = guildId != null ? (Kook as IKookClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).GetAwaiter().GetResult() : null;
        if (model.Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(model.Content, null, guild, MentionedUsers, TagMode.PlainText);
        else if (model.Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(model.Content, null, guild, MentionedUsers, TagMode.KMarkdown);
        _isMentioningEveryone = model.MentionAll;
        _isMentioningHere = model.MentionHere;
        _roleMentionIds = model.MentionRoles.ToImmutableArray();
        
        if (model.Quote is not null)
        {
            IUser refMsgAuthor = MessageHelper.GetAuthor(Kook, null, model.Quote.Author);
            _quote = Quote.Create(model.Quote.Id, model.Quote.QuotedMessageId, model.Quote.Type, model.Quote.Content, model.Quote.CreateAt, refMsgAuthor);
        }

        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);
        
        _cards = model.Type == MessageType.Card 
            ? MessageHelper.ParseCards(model.Content) 
            : ImmutableArray.Create<ICard>();
        
        _embeds = model.Embeds.Select(x => x.ToEntity()).ToImmutableArray();
    }
    
    internal override void Update(DirectMessage model)
    {
        base.Update(model);
        var guildId = (Channel as IGuildChannel)?.GuildId;
        var guild = guildId != null ? (Kook as IKookClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).GetAwaiter().GetResult() : null;
        if (model.Type == MessageType.Text)
            _tags = MessageHelper.ParseTags(model.Content, null, guild, MentionedUsers, TagMode.PlainText);
        else if (model.Type == MessageType.KMarkdown)
            _tags = MessageHelper.ParseTags(model.Content, null, guild, MentionedUsers, TagMode.KMarkdown);
        
        if (model.Quote is not null)
        {
            IUser refMsgAuthor = MessageHelper.GetAuthor(Kook, null, model.Quote.Author);
            _quote = Quote.Create(model.Quote.Id, model.Quote.QuotedMessageId, model.Quote.Type, model.Quote.Content, model.Quote.CreateAt, refMsgAuthor);
        }

        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);
        
        _cards = model.Type == MessageType.Card 
            ? MessageHelper.ParseCards(model.Content) 
            : ImmutableArray.Create<ICard>();
        
        _embeds = model.Embeds.Select(x => x.ToEntity()).ToImmutableArray();
    }
    
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
    
    #region IUserMessage
    
    /// <inheritdoc />
    public async Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
    {
        await MessageHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        MessageProperties properties = new()
        {
            Content = Content,
            Cards = Cards.Select(c => (Card) c).ToList(),
            Quote = Quote
        };
        func(properties);
        Content = properties.Content;
        _cards = properties.Cards?.Select(c => (ICard) c).ToImmutableArray() ?? ImmutableArray<ICard>.Empty;
        _quote = properties.Quote?.QuotedMessageId == Guid.Empty ? null : (Quote) properties.Quote;
    }
    
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