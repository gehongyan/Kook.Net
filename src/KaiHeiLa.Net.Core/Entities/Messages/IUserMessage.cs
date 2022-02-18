namespace KaiHeiLa;

public interface IUserMessage : IMessage
{
    IQuote Quote { get; }
}