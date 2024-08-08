namespace Kook;

/// <summary>
///     表示一个通用的私有频道，只有特定的用户可以访问。
/// </summary>
public interface IPrivateChannel : IChannel
{
    /// <summary>
    ///     获取可以访问此频道的所有用户。
    /// </summary>
    IReadOnlyCollection<IUser> Recipients { get; }
}
