using System.Collections.Generic;

namespace Kook
{
    /// <summary>
    ///     Represents the user's presence status. This may include their online status and their activity.
    /// </summary>
    public interface IPresence
    {
        /// <summary>
        ///     Gets the current status of this user.
        /// </summary>
        bool? IsOnline { get; }
        /// <summary>
        ///     Gets the type of the client where this user is currently active.
        /// </summary>
        ClientType? ActiveClient { get; }
    }
}
