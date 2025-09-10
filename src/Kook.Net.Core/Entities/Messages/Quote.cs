using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个引用的消息。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Quote : IQuote
{
    /// <inheritdoc />
    public Guid QuotedMessageId { get; }

    /// <summary>
    ///     获取此引用的消息的类型。
    /// </summary>
    public MessageType Type { get; }

    /// <summary>
    ///     获取此引用的消息的内容。
    /// </summary>
    /// <remarks>
    ///     如果此引用的消息不是文本消息，则此属性可能为空或包含原始代码。
    /// </remarks>
    public string Content { get; }

    /// <summary>
    ///     获取此引用的消息的发送时间。
    /// </summary>
    public DateTimeOffset CreateAt { get; }

    /// <summary>
    ///     获取此引用的消息的作者。
    /// </summary>
    public IUser Author { get; }

    /// <inheritdoc cref="Kook.MessageReference.Empty" />
    [Obsolete("Use MessageReference.Empty instead.")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static MessageReference Empty => new(Guid.Empty);

    /// <summary>
    ///     使用指定的消息 ID 创建一个新的 <see cref="Quote"/> 实例。
    /// </summary>
    /// <param name="quotedMessageId"> 要引用的消息的 ID。 </param>
    /// <seealso cref="Kook.MessageReference(System.Guid,System.Nullable{System.Guid})"/>
    [Obsolete("Use MessageReference instead.")]
    public Quote(Guid quotedMessageId)
    {
        QuotedMessageId = quotedMessageId;
        Content = string.Empty;
        Author = null!;
    }

    internal Quote(Guid quotedMessageId, MessageType type, string content, DateTimeOffset createAt, IUser author)
    {
        QuotedMessageId = quotedMessageId;
        Type = type;
        Content = content;
        CreateAt = createAt;
        Author = author;
    }

    internal static Quote Create(Guid quotedMessageId, MessageType type,
        string content, DateTimeOffset createAt, IUser author) =>
        new(quotedMessageId, type, content, createAt, author);

    private string DebuggerDisplay => $"{Author}: {Content} {QuotedMessageId:P}";
}
