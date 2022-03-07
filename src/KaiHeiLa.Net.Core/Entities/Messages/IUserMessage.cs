namespace KaiHeiLa;

public interface IUserMessage : IMessage
{
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
    Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null);
}