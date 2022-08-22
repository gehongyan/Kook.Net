using Kook.Utils;

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
            IGuildChannel guildChannel => $"https://https://www.kookapp.cn/app/channels/{guildChannel.GuildId}/{channel.Id}/{msg.Id}",
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
    /// IEmote A = new Emoji("🅰");
    /// IEmote B = new Emoji("🅱");
    /// await msg.AddReactionsAsync(new[] { A, B });
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
    public static async Task AddReactionsAsync(this IUserMessage msg, IEnumerable<IEmote> reactions, RequestOptions options = null)
    {
        foreach (IEmote rxn in reactions)
            await msg.AddReactionAsync(rxn, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Remove multiple reactions from a message.
    /// </summary>
    /// <remarks>
    ///     This method does not bulk remove reactions! It will send a request for each reaction included.
    /// </remarks>
    /// <example>
    /// <code language="cs">
    /// await msg.RemoveReactionsAsync(currentUser, new[] { A, B });
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
    public static async Task RemoveReactionsAsync(this IUserMessage msg, IUser user, IEnumerable<IEmote> reactions, RequestOptions options = null)
    {
        foreach (IEmote rxn in reactions)
            await msg.RemoveReactionAsync(rxn, user, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends an inline reply of plain text that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="text">The message to be sent.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyTextAsync(this IUserMessage message, 
        string text, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendTextMessageAsync(text, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of image that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="path">The file path of the image file.</param>
    /// <param name="fileName">The name of the image file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyImageAsync(this IUserMessage message, 
        string path, string fileName = null, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendImageMessageAsync(path, fileName, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of image that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="stream">Stream of the image file to be sent.</param>
    /// <param name="fileName">The name of the image file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyImageAsync(this IUserMessage message, 
        Stream stream, string fileName = null, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendImageMessageAsync(stream, fileName, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of image that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="uri">URI of the image file to be sent.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyImageAsync(this IUserMessage message, 
        Uri uri, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendImageMessageAsync(uri, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of video that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="path">The file path of the video file.</param>
    /// <param name="fileName">The name of the video file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyVideoAsync(this IUserMessage message, 
        string path, string fileName = null, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendVideoMessageAsync(path, fileName, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of video that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="stream">Stream of the video file to be sent.</param>
    /// <param name="fileName">The name of the video file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyVideoAsync(this IUserMessage message, 
        Stream stream, string fileName = null, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendVideoMessageAsync(stream, fileName, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of video that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="uri"> URI of the video file to be sent.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyVideoAsync(this IUserMessage message, 
        Uri uri, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendVideoMessageAsync(uri, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    // /// <summary>
    // ///     Sends an inline reply of audio that references a message.
    // /// </summary>
    // /// <param name="message">The message that is being replied on.</param>
    // /// <param name="path">The file path of the audio file.</param>
    // /// <param name="fileName">The name of the audio file.</param>
    // /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    // /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    // /// <param name="options">The options to be used when sending the request.</param>
    // public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyAudioAsync(this IUserMessage message, 
    //     string path, string fileName = null, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    // {
    //     return await message.Channel.SendAudioMessageAsync(path, fileName, isQuote ? new Quote(message.Id) : null,
    //         isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    // }
    // /// <summary>
    // ///     Sends an inline reply of audio that references a message.
    // /// </summary>
    // /// <param name="message">The message that is being replied on.</param>
    // /// <param name="stream">Stream of the audio file to be sent.</param>
    // /// <param name="fileName">The name of the audio file.</param>
    // /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    // /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    // /// <param name="options">The options to be used when sending the request.</param>
    // public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyAudioAsync(this IUserMessage message, 
    //     Stream stream, string fileName = null, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    // {
    //     return await message.Channel.SendAudioMessageAsync(stream, fileName, isQuote ? new Quote(message.Id) : null,
    //         isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    // }
    // /// <summary>
    // ///     Sends an inline reply of audio that references a message.
    // /// </summary>
    // /// <param name="message">The message that is being replied on.</param>
    // /// <param name="uri"> URI of the audio file to be sent.</param>
    // /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    // /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    // /// <param name="options">The options to be used when sending the request.</param>
    // public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyAudioAsync(this IUserMessage message, 
    //     Uri uri, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    // {
    //     return await message.Channel.SendAudioMessageAsync(uri, isQuote ? new Quote(message.Id) : null,
    //         isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    // }
    /// <summary>
    ///     Sends an inline reply of file that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="path">The file path of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyFileAsync(this IUserMessage message, 
        string path, string fileName = null, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendFileMessageAsync(path, fileName, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of file that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="stream">Stream of the file to be sent.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyFileAsync(this IUserMessage message, 
        Stream stream, string fileName = null, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendFileMessageAsync(stream, fileName, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of file that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="uri">URI of the file to be sent.</param>
    /// <param name="isQuote"> <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>. </param>
    /// <param name="isEphemeral"> <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>. </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyFileAsync(this IUserMessage message, 
        Uri uri, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendFileMessageAsync(uri, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends an inline reply of KMarkdown that references a message.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="content">Contents of the message.</param>
    /// <param name="isQuote"><c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.</param>
    /// <param name="isEphemeral"><c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.</param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyKMarkdownAsync(this IUserMessage message, 
        string content, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendKMarkdownMessageAsync(content, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends a card message to the source channel.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="cards">The cards to be sent.</param>
    /// <param name="isQuote"><c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.</param>
    /// <param name="isEphemeral"><c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.</param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyCardsAsync(this IUserMessage message, 
        IEnumerable<ICard> cards, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendCardMessageAsync(cards, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends a card message to the source channel.
    /// </summary>
    /// <param name="message">The message that is being replied on.</param>
    /// <param name="card">The card to be sent.</param>
    /// <param name="isQuote"><c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.</param>
    /// <param name="isEphemeral"><c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.</param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyCardsAsync(this IUserMessage message, 
        ICard card, bool isQuote = false, bool isEphemeral = false, RequestOptions options = null)
    {
        return await message.Channel.SendCardMessageAsync(new[] { card }, isQuote ? new Quote(message.Id) : null,
            isEphemeral ? message.Author : null, options).ConfigureAwait(false);
    }
        
}