using System.Diagnostics;
using Model = KaiHeiLa.API.Quote;

namespace KaiHeiLa;

public class Quote : IQuote
{
    public Guid Id { get; internal set; }
    public MessageType Type { get; internal set; }
    public string Content { get; internal set; }
    public DateTimeOffset CreateAt { get; internal set; }
    public IUser Author { get; internal set; }

    internal Quote(Guid id, MessageType type, string content, DateTimeOffset createAt, IUser author)
    {
        Id = id;
        Type = type;
        Content = content;
        CreateAt = createAt;
        Author = author;
    }

    internal static Quote Create(Model model, IUser author)
        => new Quote(model.Id, model.Type, model.Content, model.CreateAt, author);
    
}