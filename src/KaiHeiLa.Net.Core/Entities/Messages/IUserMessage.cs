namespace KaiHeiLa;

public interface IUserMessage : IMessage
{
    IMessage Quote { get; }
    
    IUser Author { get; }
    
    IGuild Guild { get; }
}