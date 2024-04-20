namespace Kook;

/// <summary>
///     Represents a generic message sent by a user.
/// </summary>
public interface IUserMessage : IMessage
{
    /// <summary>
    ///     Gets the message quote.
    /// </summary>
    /// <returns>
    ///     The message quote.
    /// </returns>
    IQuote Quote { get; }

    /// <summary>
    ///     Modifies this message.
    /// </summary>
    /// <remarks>
    ///     This method modifies this message with the specified properties. To see an example of this
    ///     method and what properties are available, please refer to <see cref="MessageProperties"/>.
    /// </remarks>
    /// <param name="func">A delegate containing the properties to modify the message with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    Task ModifyAsync(Action<MessageProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     Transforms this message's text into a human-readable form by resolving its tags.
    /// </summary>
    /// <param name="userHandling">Determines how the user tag should be handled.</param>
    /// <param name="channelHandling">Determines how the channel tag should be handled.</param>
    /// <param name="roleHandling">Determines how the role tag should be handled.</param>
    /// <param name="everyoneHandling">Determines how the @everyone tag should be handled.</param>
    /// <param name="emojiHandling">Determines how the emoji tag should be handled.</param>
    string Resolve(
        TagHandling userHandling = TagHandling.Name,
        TagHandling channelHandling = TagHandling.Name,
        TagHandling roleHandling = TagHandling.Name,
        TagHandling everyoneHandling = TagHandling.Name,
        TagHandling emojiHandling = TagHandling.Name);
}
