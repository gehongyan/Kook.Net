namespace Kook;

/// <summary>
///     Properties that are used to modify an <see cref="IUserMessage" /> with the specified changes.
/// </summary>
/// <seealso cref="IUserMessage.ModifyAsync"/>
public class MessageProperties
{
    /// <summary>
    ///     Gets or sets the content of the message.
    /// </summary>
    /// <remarks>
    ///     This must be less than the constant defined by <see cref="KookConfig.MaxMessageSize"/>.
    /// </remarks>
    public string Content { get; set; }

    /// <summary>
    ///     Gets or sets the cards of the message.
    /// </summary>
    public IEnumerable<ICard> Cards { get; set; }

    /// <summary>
    ///     Gets or sets the quote of the message.
    /// </summary>
    public IQuote Quote { get; set; }

    /// <summary>
    ///     Gets or sets the only user that can see this message.
    /// </summary>
    public IUser EphemeralUser { get; set; }
}
