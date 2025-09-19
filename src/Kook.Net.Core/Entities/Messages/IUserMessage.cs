namespace Kook;

/// <summary>
///     表示一个通用的用户消息。
/// </summary>
public interface IUserMessage : IMessage
{
    /// <summary>
    ///     获取消息的引用。
    /// </summary>
    IQuote? Quote { get; }

    /// <summary>
    ///     修改此消息。
    /// </summary>
    /// <param name="func"> 一个包含修改消息属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    /// <seealso cref="Kook.MessageProperties"/>
    Task ModifyAsync(Action<MessageProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     修改此消息。
    /// </summary>
    /// <param name="func"> 一个包含修改消息属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <typeparam name="T"> 消息的内容类型。 </typeparam>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    /// <seealso cref="Kook.MessageProperties"/>
    Task ModifyAsync<T>(Action<MessageProperties<T>> func, RequestOptions? options = null);

    /// <summary>
    ///     置顶此消息到其频道的置顶消息列表中。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步置顶操作的任务。 </returns>
    Task PinAsync(RequestOptions? options = null);

    /// <summary>
    ///     从其频道的置顶消息列表中移除此消息。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除操作的任务。 </returns>
    Task UnpinAsync(RequestOptions? options = null);

    /// <summary>
    ///     转换消息文本中的提及与表情符号为可读形式。
    /// </summary>
    /// <param name="userHandling"> 指定用户提及标签的处理方式。 </param>
    /// <param name="channelHandling"> 指定频道提及标签的处理方式。 </param>
    /// <param name="roleHandling"> 指定角色提及标签的处理方式。 </param>
    /// <param name="everyoneHandling"> 指定全体成员与在线成员提及标签的处理方式。 </param>
    /// <param name="emojiHandling"> 指定表情符号标签的处理方式。 </param>
    /// <returns> 转换后的消息文本。 </returns>
    /// <remarks>
    ///     此方法推荐适用于消息类型为 <see cref="Kook.MessageType.Text"/> 或
    ///     <see cref="Kook.MessageType.KMarkdown"/>，对于 <see cref="Kook.MessageType.Card"/> 类型的消息，参见
    ///     <see cref="Kook.MessageExtensions.TryExtractCardContent(Kook.IUserMessage,out System.String)"/>。
    /// </remarks>
    string Resolve(
        TagHandling userHandling = TagHandling.Name,
        TagHandling channelHandling = TagHandling.Name,
        TagHandling roleHandling = TagHandling.Name,
        TagHandling everyoneHandling = TagHandling.Name,
        TagHandling emojiHandling = TagHandling.Name);
}
