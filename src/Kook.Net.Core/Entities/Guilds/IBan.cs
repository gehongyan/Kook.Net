namespace Kook;

/// <summary>
///     表示一个通用的封禁对象。
/// </summary>
public interface IBan
{
    /// <summary>
    ///     获取被封禁的用户。
    /// </summary>
    IUser User { get; }

    /// <summary>
    ///     获取封禁的时间。
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     获取封禁的原因。
    /// </summary>
    string Reason { get; }
}
