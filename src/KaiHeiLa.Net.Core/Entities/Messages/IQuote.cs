namespace KaiHeiLa;

public interface IQuote
{
    string Id { get; }
    Guid QuotedMessageId { get; }
    MessageType Type { get; }
    string Content { get; }
    DateTimeOffset CreateAt { get; }
    IUser Author { get; }
}