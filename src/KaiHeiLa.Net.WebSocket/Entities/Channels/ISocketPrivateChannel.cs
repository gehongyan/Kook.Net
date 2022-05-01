using System.Collections.Generic;

namespace KaiHeiLa.WebSocket
{
    /// <summary>
    ///     Represents a generic WebSocket-based channel that is private to select recipients.
    /// </summary>
    public interface ISocketPrivateChannel : IPrivateChannel
    {
        /// <summary>
        ///     Gets the users that can access this channel.
        /// </summary>
        /// <returns>
        ///     A read-only collection of users that can access this channel.
        /// </returns>
        new IReadOnlyCollection<SocketUser> Recipients { get; }
    }
}
