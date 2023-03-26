namespace Kook;

/// <summary>
///     Represents a source from which a friend request is sent.
/// </summary>
public enum RequestFriendSource
{
    /// <summary>
    ///     Represents the friend request is sent via the user's full qualification,
    ///     such as <c>username#1234</c>.
    /// </summary>
    FullQualification = 0,

    /// <summary>
    ///     Represents the friend request is sent via a guild where the user and
    ///     the current user are in.
    /// </summary>
    Guild = 2
}
