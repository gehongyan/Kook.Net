namespace Kook;

/// <summary>
///     表示一个通用的消息引用。
/// </summary>
public interface IQuote
{
    /// <summary>
    ///     获取此消息引用所引用的消息的 ID。
    /// </summary>
    Guid QuotedMessageId { get; }
}
