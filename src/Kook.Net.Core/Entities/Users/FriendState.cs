namespace Kook;

/// <summary>
///     Represents a state of a friend relationship with the current user.
/// </summary>
public enum FriendState
{
    /// <summary>
    ///     Represents a pending friend request that has not been accepted yet.
    /// </summary>
    Pending,

    /// <summary>
    ///     Represents an accepted friend request, where the user has been added to the current user's friend list.
    /// </summary>
    Accepted,

    /// <summary>
    ///     Represents a blocked friend status, where the user has been blocked by the current user.
    /// </summary>
    Blocked
}
