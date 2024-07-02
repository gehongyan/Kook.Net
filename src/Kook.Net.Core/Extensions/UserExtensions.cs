namespace Kook;

/// <summary> An extension class for various Kook user objects. </summary>
public static class UserExtensions
{
    /// <summary>
    ///     Sends a file via DM.
    /// </summary>
    /// <param name="user">The user to send the DM to.</param>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
        string path, string? filename = null, AttachmentType type = AttachmentType.File, IQuote? quote = null,
        RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendFileAsync(path, filename, type, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a file via DM.
    /// </summary>
    /// <param name="user">The user to send the DM to.</param>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
        Stream stream, string filename, AttachmentType type = AttachmentType.File, IQuote? quote = null,
        RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendFileAsync(stream, filename, type, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a file via DM.
    /// </summary>
    /// <param name="user">The user to send the DM to.</param>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
        FileAttachment attachment, IQuote? quote = null, RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendFileAsync(attachment, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a text message via DM.
    /// </summary>
    /// <param name="user">The user to send the DM to.</param>
    /// <param name="content">The KMarkdown content to be sent.</param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> SendTextAsync(this IUser user,
        string content, IQuote? quote = null, RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendTextAsync(content, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a card message message via DM.
    /// </summary>
    /// <param name="user">The user to send the DM to.</param>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(this IUser user,
        IEnumerable<ICard> cards, IQuote? quote = null, RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendCardsAsync(cards, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a card message message via DM.
    /// </summary>
    /// <param name="user">The user to send the DM to.</param>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public static async Task<Cacheable<IUserMessage, Guid>> SendCardAsync(this IUser user,
        ICard card, IQuote? quote = null, RequestOptions? options = null)
    {
        IDMChannel dmChannel = await user.CreateDMChannelAsync().ConfigureAwait(false);
        return await dmChannel.SendCardAsync(card, quote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Bans the user from the guild and optionally prunes their recent messages.
    /// </summary>
    /// <param name="user">The user to ban.</param>
    /// <param name="pruneDays">The number of days to remove messages from this <paramref name="user"/> for - must be between [0, 7]</param>
    /// <param name="reason">The reason of the ban to be written in the audit log.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <exception cref="ArgumentException"><paramref name="pruneDays" /> is not between 0 to 7.</exception>
    /// <returns>
    ///     A task that represents the asynchronous operation for banning a user.
    /// </returns>
    public static Task BanAsync(this IGuildUser user, int pruneDays = 0, string? reason = null,
        RequestOptions? options = null) =>
        user.Guild.AddBanAsync(user, pruneDays, reason, options);
}
