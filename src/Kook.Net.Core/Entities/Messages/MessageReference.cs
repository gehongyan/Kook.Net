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
    /// <seealso cref="Kook.MessageProperties.Quote"/>
    public static MessageReference Empty => new();

    /// <summary>
    ///     使用指定的消息 ID 创建一个新的 <see cref="MessageReference"/> 实例。
    /// </summary>
    /// <param name="quotedMessageId">
    ///     要回复的消息的 ID。当此值为 <c>default</c> 时，将不引用回复任何消息。
    /// </param>
    /// <param name="replyMessageId">
    ///     要引用的消息的 ID，用于每日消息可发送条数总量消耗折扣，在
    ///     <see cref="Kook.KookConfig.MessageReplyDiscountTimeSpan"/> 内回复有效。当此值为 <c>null</c> 时，将使用
    ///     <paramref name="quotedMessageId"/> 的值；此值为 <c>default</c> 时，将不引用任何消息。
    /// </param>
    /// <example>
    ///     引用回复并自动应用每日消息可发送条数总量消耗的折扣：
    ///     <code>
    ///     new MessageReference(quotedMessageId);
    ///     </code>
    ///     不引用回复但需要应用每日消息可发送条数总量消耗的折扣：
    ///     <code>
    ///     new MessageReference(replyMessageId: replyMessageId);
    ///     </code>
    ///     需要应用每日消息可发送条数总量消耗的折扣，但需要同时回复另一条消息：
    ///     <code>
    ///     new MessageReference(quotedMessageId, replyMessageId);
    ///     </code>
    /// </example>
    public MessageReference(Guid quotedMessageId = default, Guid? replyMessageId = null)
    {
        QuotedMessageId = quotedMessageId;
        ReplyMessageId = replyMessageId ?? quotedMessageId;
    }

    /// <inheritdoc />
    public Guid QuotedMessageId { get; }

    /// <summary>
    ///     获取此消息引用所回复的消息的 ID，用于每日消息可发送条数总量消耗折扣，在
    ///     <see cref="Kook.KookConfig.MessageReplyDiscountTimeSpan"/> 内回复有效。
    /// </summary>
    public Guid ReplyMessageId { get; }

    private string DebuggerDisplay => $"Reference: {QuotedMessageId}, Reply: {ReplyMessageId}";
}
