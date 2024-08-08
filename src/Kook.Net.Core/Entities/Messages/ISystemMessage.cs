namespace Kook;

/// <summary>
///     表示一个通用的系统消息。
/// </summary>
/// <remarks>
///     <note type="warning">
///         KOOK 未统一规范系统消息类型，此接口并未在 <see cref="T:Kook.IMessage"/> 之上封装更多的实用功能。
///     </note>
/// </remarks>
public interface ISystemMessage : IMessage
{
    /// <summary>
    ///     获取此系统消息的类型。
    /// </summary>
    SystemMessageType SystemMessageType { get; }
}
