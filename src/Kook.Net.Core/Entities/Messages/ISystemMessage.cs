namespace Kook;

/// <summary>
///     Represents a generic message sent by the system.
/// </summary>
public interface ISystemMessage : IMessage
{
    /// <summary>
    ///     Gets the type of the system message.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         Because the data from Kook does not describe the type
    ///         of the message in detail, this property is not implemented yet.
    ///         Accessing this property will always result in an exception at present.
    ///     </note>
    /// </remarks>
    SystemMessageType SystemMessageType { get; }
}
