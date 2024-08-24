namespace Kook;

/// <summary>
///     表示一个通用的好友请求。
/// </summary>
public interface IFriendRequest : IEntity<ulong>
{
    /// <summary>
    ///     获取发送此好友请求的用户。
    /// </summary>
    IUser User { get; }

    /// <summary>
    ///     接受此好友请求。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步接受操作的任务。 </returns>
    Task AcceptAsync(RequestOptions? options = null);

    /// <summary>
    ///     拒绝此好友请求。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步拒绝操作的任务。 </returns>
    Task DeclineAsync(RequestOptions? options = null);
}
