namespace Kook;

/// <summary>
///     表示一个通用的消息引用。
/// </summary>
public interface IQuote
{
    /// <summary>
    ///     获取此引用所指向的消息的 ID。
    /// </summary>
    Guid QuotedMessageId { get; }
}
