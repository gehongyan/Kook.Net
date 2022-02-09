namespace KaiHeiLa;

public interface IQuote
{
    Guid Id { get; }
    MessageType Type { get; }
    string Content { get; }
    DateTimeOffset CreateAt { get; }
    IUser Author { get; }
}