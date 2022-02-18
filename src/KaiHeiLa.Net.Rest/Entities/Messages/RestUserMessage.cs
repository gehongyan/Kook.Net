using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using KaiHeiLa.API;
using KaiHeiLa.Net.Converters;
using Model = KaiHeiLa.API.Message;

namespace KaiHeiLa.Rest;

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
    private ImmutableArray<uint> _roleMentionIds = ImmutableArray.Create<uint>();
    
    public Quote Quote => _quote;
    
    /// <inheritdoc />
    public override Attachment Attachment => _attachment;
    /// <inheritdoc />  
    public override IReadOnlyCollection<ICard> Cards => _cards;
    /// <inheritdoc />
    public override bool? MentionedEveryone => _isMentioningEveryone;
    /// <inheritdoc />
    public override bool? MentionedHere => _isMentioningHere;
    /// <inheritdoc />
    public override IReadOnlyCollection<uint> MentionedRoleIds => _roleMentionIds;
    
    internal RestUserMessage(BaseKaiHeiLaClient kaiHeiLa, Guid id, MessageType messageType, IMessageChannel channel, IUser author, MessageSource source)
        : base(kaiHeiLa, id, messageType, channel, author, source)
    {
    }
    internal new static RestUserMessage Create(BaseKaiHeiLaClient kaiHeiLa, IMessageChannel channel, IUser author, Model model)
    {
        var entity = new RestUserMessage(kaiHeiLa, model.Id, model.Type, channel, author, MessageHelper.GetSource(model));
        entity.Update(model);
        return entity;
    }

    internal override void Update(Model model)
    {
        base.Update(model);
        _isMentioningEveryone = model.MentionAll;
        _roleMentionIds = model.MentionRoles.ToImmutableArray();
        
        if (model.Quote is not null)
        {
            IUser refMsgAuthor = MessageHelper.GetAuthor(KaiHeiLa, null, model.Quote.Author);
            _quote = Quote.Create(model.Quote, refMsgAuthor);
        }

        if (model.Attachment is not null)
            _attachment = Attachment.Create(model.Attachment);
        
        if (model.Type == MessageType.Card)
        {
            string json = model.Content;
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
    
    
    #region IUserMessage

    IQuote IUserMessage.Quote => _quote;
    IReadOnlyCollection<ICard> IMessage.Cards => Cards;

    #endregion
}