namespace Kook;

/// <summary>
///     Provides extension methods for <see cref="IMessage" />.
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    ///     Gets a URL that jumps to the message.
    /// </summary>
    /// <param name="msg">The message to jump to.</param>
    /// <returns>
    ///     A string that contains a URL for jumping to the message in chat.
    /// </returns>
    public static string GetJumpUrl(this IMessage msg)
    {
        IMessageChannel channel = msg.Channel;
        return channel switch
        {
            IDMChannel dmChannel => $"https://www.kookapp.cn/app/home/privatemessage/{dmChannel.Recipient.Id}/{msg.Id}",
            IGuildChannel guildChannel => $"https://www.kookapp.cn/app/channels/{guildChannel.GuildId}/{channel.Id}/{msg.Id}",
            _ => throw new ArgumentException("Message must be in a guild or a DM channel.", nameof(msg))
        };
    }

    /// <summary>
    ///     Add multiple reactions to a message.
    /// </summary>
    /// <remarks>
    ///     This method does not bulk add reactions! It will send a request for each reaction included.
    /// </remarks>
    /// <example>
    /// <code language="cs">
    /// IEmote a = new Emoji("🅰");
    /// IEmote b = new Emoji("🅱");
    /// await msg.AddReactionsAsync([a, b]);
    /// </code>
    /// </example>
    /// <param name="msg">The message to add reactions to.</param>
    /// <param name="reactions">An array of reactions to add to the message.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous operation for adding a reaction to this message.
    /// </returns>
    /// <seealso cref="IMessage.AddReactionAsync(IEmote, RequestOptions)"/>
    /// <seealso cref="IEmote"/>
    public static async Task AddReactionsAsync(this IUserMessage msg, IEnumerable<IEmote> reactions,
        RequestOptions? options = null)
    {
        foreach (IEmote emote in reactions)
            await msg.AddReactionAsync(emote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Remove multiple reactions from a message.
    /// </summary>
    /// <remarks>
    ///     This method does not bulk remove reactions! It will send a request for each reaction included.
    /// </remarks>
    /// <example>
    /// <code language="cs">
    /// await msg.RemoveReactionsAsync(currentUser, [A, B]);
    /// </code>
    /// </example>
    /// <param name="msg">The message to remove reactions from.</param>
    /// <param name="user">The user who removed the reaction.</param>
    /// <param name="reactions">An array of reactions to remove from the message.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous operation for removing a reaction to this message.
    /// </returns>
    /// <seealso cref="IMessage.RemoveReactionAsync(IEmote, IUser, RequestOptions)"/>
    /// <seealso cref="IEmote"/>
    public static async Task RemoveReactionsAsync(this IUserMessage msg, IUser user, IEnumerable<IEmote> reactions,
        RequestOptions? options = null)
    {
        foreach (IEmote emote in reactions)
            await msg.RemoveReactionAsync(emote, user, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends an inline reply of file that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
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
    ///     Sends an inline reply of file that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
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
    ///     Sends an inline reply of file that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        FileAttachment attachment, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(attachment,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     Sends an inline reply of text that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="content">Contents of the message.</param>
    /// <param name="isQuote"><c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.</param>
    /// <param name="isEphemeral"><c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyTextAsync(this IUserMessage message,
        string content, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendTextAsync(content,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     Sends a card message to the source channel.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="isQuote"><c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.</param>
    /// <param name="isEphemeral"><c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyCardsAsync(this IUserMessage message,
        IEnumerable<ICard> cards, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendCardsAsync(cards,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     Sends a card message to the source channel.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="isQuote"><c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.</param>
    /// <param name="isEphemeral"><c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyCardAsync(this IUserMessage message,
        ICard card, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendCardAsync(card,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     Gets whether the message may be a text image mixed message.
    /// </summary>
    /// <param name="msg"> The message to check against. </param>
    /// <returns> <c>true</c> if the message may be a text image mixed message; otherwise, <c>false</c>. </returns>
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
