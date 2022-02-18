namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based channel that can send and receive messages.
/// </summary>
public interface IRestMessageChannel : IMessageChannel
{
    /// <summary>
    ///     Gets a message from this message channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessageAsync(Guid, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="id">The identifier of the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
    ///     the retrieved message; <c>null</c> if no message is found with the specified identifier.
    /// </returns>
    Task<RestMessage> GetMessageAsync(Guid id, RequestOptions options = null);
}