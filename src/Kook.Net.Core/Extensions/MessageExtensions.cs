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
    /// <param name="options">The options to be used when sending the request.</param>
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
    /// <param name="options">The options to be used when sending the request.</param>
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
    /// <param name="path">The file path of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="type">The type of the file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        string path, string? fileName = null, AttachmentType type = AttachmentType.File, bool isQuote = false,
        bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(path, fileName, type,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     Sends an inline reply of file that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="stream">Stream of the file to be sent.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="type">The type of the file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        Stream stream, string? fileName = null, AttachmentType type = AttachmentType.File, bool isQuote = false,
        bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(stream, fileName, type,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     Sends an inline reply of file that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="attachment">The attachment containing the file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
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
    /// <param name="options">The request options for this <c>async</c> request.</param>
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
    /// <param name="cards">The cards to be sent.</param>
    /// <param name="isQuote"><c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.</param>
    /// <param name="isEphemeral"><c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.</param>
    /// <param name="options">The request options for this <c>async</c> request.</param>
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
    /// <param name="card">The card to be sent.</param>
    /// <param name="isQuote"><c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.</param>
    /// <param name="isEphemeral"><c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.</param>
    /// <param name="options">The request options for this <c>async</c> request.</param>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyCardAsync(this IUserMessage message,
        ICard card, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendCardAsync(card,
                isQuote ? new MessageReference(message.Id) : null,
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);
}
