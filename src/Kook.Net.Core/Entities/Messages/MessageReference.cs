using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个消息引用。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class MessageReference : IQuote
{
    /// <summary>
    ///     获取一个引用的消息为空的引用。
    /// </summary>
    /// <remarks>
    ///     此属性用于在修改消息时删除引用。
    /// </remarks>
    /// <seealso cref="P:Kook.MessageProperties.Quote"/>
    public static MessageReference Empty => new(Guid.Empty);

    /// <summary>
    ///     使用指定的消息 ID 创建一个新的 <see cref="MessageReference"/> 实例。
    /// </summary>
    /// <param name="quotedMessageId"> 要引用的消息的 ID。 </param>
    public MessageReference(Guid quotedMessageId)
    {
        QuotedMessageId = quotedMessageId;
    }

    /// <inheritdoc />
    public Guid QuotedMessageId { get; }

    private string DebuggerDisplay => $"Quote: {QuotedMessageId}";
}
