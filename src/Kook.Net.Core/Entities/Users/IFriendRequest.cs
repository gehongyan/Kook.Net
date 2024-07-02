namespace Kook;

/// <summary>
///     Represents a generic friend request.
/// </summary>
public interface IFriendRequest : IEntity<ulong>
{
    /// <summary>
    ///     Gets the user who sent this friend request.
    /// </summary>
    IUser User { get; }

    /// <summary>
    ///     Accepts this friend request.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task that represents the asynchronous accept operation. </returns>
    Task AcceptAsync(RequestOptions? options = null);

    /// <summary>
    ///     Declines this friend request.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task that represents the asynchronous decline operation. </returns>
    Task DeclineAsync(RequestOptions? options = null);
}
