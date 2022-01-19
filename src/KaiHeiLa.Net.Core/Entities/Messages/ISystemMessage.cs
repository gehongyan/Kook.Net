namespace KaiHeiLa;

public interface ISystemMessage : IMessage
{
    SystemMessageType SystemMessageType { get; }
}