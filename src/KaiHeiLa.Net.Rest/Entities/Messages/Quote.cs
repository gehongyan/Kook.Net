using System.Diagnostics;
using Model = KaiHeiLa.API.Quote;

namespace KaiHeiLa;

public class Quote : IQuote
{
    public string Id { get; internal set; }
    public Guid QuotedMessageId { get; internal set; }
    public MessageType Type { get; internal set; }
    public string Content { get; internal set; }
    public DateTimeOffset CreateAt { get; internal set; }
    public IUser Author { get; internal set; }

    public Quote(Guid quotedMessageId)
    {
        QuotedMessageId = quotedMessageId;
    }
    
    internal Quote(string id, Guid quotedMessageId, MessageType type, string content, DateTimeOffset createAt, IUser author)
    {
        Id = id;
        QuotedMessageId = quotedMessageId;
        Type = type;
        Content = content;
        CreateAt = createAt;
        Author = author;
    }

    internal static Quote Create(Model model, IUser author)
        => new Quote(model.Id, model.QuotedMessageId, model.Type, model.Content, model.CreateAt, author);
    
}