namespace Kook;

/// <summary>
///     表示一个通用的私聊频道。
/// </summary>
public interface IDMChannel : IMessageChannel, IPrivateChannel, IEntity<Guid>
{
    #region General

    /// <summary>
    ///     获取此私聊频道的唯一标识符。
    /// </summary>
    new Guid Id { get; }

    /// <summary>
    ///     获取此私聊频道的聊天代码。
    /// </summary>
    Guid ChatCode { get; }

    /// <summary>
    ///     获取参与到此私聊频道的另外一位用户。
    /// </summary>
    IUser Recipient { get; }

    /// <summary>
    ///     关闭此私聊频道，将其从您的频道列表中移除。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步关闭操作的任务。 </returns>
    Task CloseAsync(RequestOptions? options = null);

    #endregion

    #region Send Messages

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null);

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, RequestOptions? options = null);

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment, IQuote? quote = null, RequestOptions? options = null);

    /// <summary>
    ///     发送文本消息到此消息频道。
    /// </summary>
    /// <param name="text"> 要发送的文本。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text, IQuote? quote = null, RequestOptions? options = null);

    /// <summary>
    ///     发送卡片消息到此消息频道。
    /// </summary>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card, IQuote? quote = null, RequestOptions? options = null);

    /// <summary>
    ///     发送卡片消息到此消息频道。
    /// </summary>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards, IQuote? quote = null, RequestOptions? options = null);

    #endregion
}
