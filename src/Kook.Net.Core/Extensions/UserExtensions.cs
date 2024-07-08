namespace Kook;

/// <summary>
///     提供用于各种用户实体的扩展方法。
/// </summary>
public static class UserExtensions
{
    /// <summary>
    ///     通过私聊发送文件。
    /// </summary>
    /// <param name="user"> 要发送消息的用户。 </param>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
        string path, string? filename = null, AttachmentType type = AttachmentType.File, IQuote? quote = null,
        RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendFileAsync(path, filename, type, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     通过私聊发送文件。
    /// </summary>
    /// <param name="user"> 要发送消息的用户。 </param>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
        Stream stream, string filename, AttachmentType type = AttachmentType.File, IQuote? quote = null,
        RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendFileAsync(stream, filename, type, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     通过私聊发送文件。
    /// </summary>
    /// <param name="user"> 要发送消息的用户。 </param>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
        FileAttachment attachment, IQuote? quote = null, RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendFileAsync(attachment, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     通过私聊发送文本消息。
    /// </summary>
    /// <param name="user"> 要发送消息的用户。 </param>
    /// <param name="content">The KMarkdown content to be sent.</param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> SendTextAsync(this IUser user,
        string content, IQuote? quote = null, RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendTextAsync(content, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     通过私聊发送卡片消息。
    /// </summary>
    /// <param name="user"> 要发送消息的用户。 </param>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(this IUser user,
        IEnumerable<ICard> cards, IQuote? quote = null, RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendCardsAsync(cards, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     通过私聊发送卡片消息。
    /// </summary>
    /// <param name="user"> 要发送消息的用户。 </param>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> SendCardAsync(this IUser user,
        ICard card, IQuote? quote = null, RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendCardAsync(card, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     封禁服务器内的用户。
    /// </summary>
    /// <param name="user"> 要封禁的用户。 </param>
    /// <param name="pruneDays"> 要删除此服务器中来自此用户的最近几天的消息，范围为 <c>0</c> 至 <c>7</c>，<c>0</c> 表示不删除。 </param>
    /// <param name="reason"> 封禁原因。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <exception cref="ArgumentException"> <paramref name="pruneDays" /> 超出了 0 至 7 的范围。 </exception>
    /// <returns>
    ///     一个表示异步封禁操作的任务。
    /// </returns>
    public static Task BanAsync(this IGuildUser user, int pruneDays = 0, string? reason = null,
        RequestOptions? options = null) =>
        user.Guild.AddBanAsync(user, pruneDays, reason, options);
}
