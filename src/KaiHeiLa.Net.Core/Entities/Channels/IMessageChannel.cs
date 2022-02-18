namespace KaiHeiLa;

public interface IMessageChannel : IChannel
{
    /// <summary>
    ///     Gets a message from this message channel.
    /// </summary>
    /// <param name="id">The identifier of the message.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
    ///     the retrieved message; <c>null</c> if no message is found with the specified identifier.
    /// </returns>
    Task<IMessage> GetMessageAsync(Guid id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

}