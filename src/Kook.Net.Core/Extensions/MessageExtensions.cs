namespace Kook;

/// <summary>
///     提供用于各种消息实体的扩展方法。
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    ///     获取一个跳转到消息的 URL。
    /// </summary>
    /// <param name="msg"> 要获取跳转 URL 的消息。 </param>
    /// <returns> 一个包含用于在聊天中跳转到消息的 URL 的字符串。 </returns>
    public static string GetJumpUrl(this IMessage msg)
    {
        IMessageChannel channel = msg.Channel;
        return channel switch
        {
            IDMChannel dmChannel => $"https://www.kookapp.cn/app/home/privatemessage/{dmChannel.ChatCode}/{msg.Id}",
            IGuildChannel guildChannel => $"https://www.kookapp.cn/direct/anchor/{guildChannel.GuildId}/{channel.Id}/{msg.Id}",
            _ => throw new ArgumentException("Message must be in a guild or a DM channel.", nameof(msg))
        };
    }

    /// <summary>
    ///     向消息添加多个回应。
    /// </summary>
    /// <remarks>
    ///     此方法会对每个要添加的回应分别发送请求。
    /// </remarks>
    /// <param name="msg"> 要添加回应的消息。 </param>
    /// <param name="reactions"> 要用于向此消息添加回应的所有表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步添加操作的任务。 </returns>
    /// <seealso cref="M:Kook.IMessage.AddReactionAsync(Kook.IEmote,Kook.RequestOptions)"/>
    public static async Task AddReactionsAsync(this IUserMessage msg, IEnumerable<IEmote> reactions,
        RequestOptions? options = null)
    {
        foreach (IEmote emote in reactions)
            await msg.AddReactionAsync(emote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     移除消息上的多个回应。
    /// </summary>
    /// <remarks>
    ///     此方法会对每个要移除的回应分别发送请求。
    /// </remarks>
    /// <param name="msg"> 要添加回应的消息。 </param>
    /// <param name="user"> 要删除其回应的用户。 </param>
    /// <param name="reactions"> 要从此消息移除的回应的所有表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除操作的任务。 </returns>
    /// <seealso cref="M:Kook.IMessage.RemoveReactionAsync(Kook.IEmote,Kook.IUser,Kook.RequestOptions)"/>
    public static async Task RemoveReactionsAsync(this IUserMessage msg, IUser user, IEnumerable<IEmote> reactions,
        RequestOptions? options = null)
    {
        foreach (IEmote emote in reactions)
            await msg.RemoveReactionAsync(emote, user, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     向消息所属的频道回复文件消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="isQuote"> 是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        string path, string? filename = null, AttachmentType type = AttachmentType.File, bool isQuote = false,
        bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(path, filename, type,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复文件消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="isQuote"> 是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        Stream stream, string filename, AttachmentType type = AttachmentType.File, bool isQuote = false,
        bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(stream, filename, type,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复文件消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="isQuote"> 是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        FileAttachment attachment, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(attachment,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复文字消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="content">Contents of the message.</param>
    /// <param name="isQuote">是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral">是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyTextAsync(this IUserMessage message,
        string content, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendTextAsync(content,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复卡片消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="isQuote">是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral">是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyCardsAsync(this IUserMessage message,
        IEnumerable<ICard> cards, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendCardsAsync(cards,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复卡片消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="isQuote">是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral">是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyCardAsync(this IUserMessage message,
        ICard card, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendCardAsync(card,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     获取此消息是否可能是图文混排消息。
    /// </summary>
    /// <param name="msg"> 要判断的消息。 </param>
    /// <returns> 如果此消息可能是图文混排消息，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool MaybeTextImageMixedMessage(this IUserMessage msg)
    {
        if (msg.Cards.Count != 1) return false;
        if (msg.Cards.First() is not Card card) return false;
        if (card.Theme != CardTheme.Invisible) return false;
        if (card.Modules.Length == 0) return false;
        return card.Modules.All(x => x is
            SectionModule { Text: PlainTextElement or KMarkdownElement }
            or ContainerModule { Elements: [not null] });
    }
}
