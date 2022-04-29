namespace KaiHeiLa;

/// <summary>
///     Represents a generic message sent by the system.
/// </summary>
public interface ISystemMessage : IMessage
{
    /// <summary>
    ///     Gets the type of the system message.
    /// </summary>
    SystemMessageType SystemMessageType { get; }
}