namespace Kook;

/// <summary>
///     表示一个通用的用户实时状态。
/// </summary>
public interface IPresence
{
    /// <summary>
    ///     获取此用户当前是否在线。
    /// </summary>
    bool? IsOnline { get; }

    /// <summary>
    ///     获取此用于当前登录的客户端类型。
    /// </summary>
    ClientType? ActiveClient { get; }
}
