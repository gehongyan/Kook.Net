namespace Kook;

/// <summary>
///     表示一个 <see cref="CountdownModule"/> 的倒计时显示模式。
/// </summary>
public enum CountdownMode
{
    /// <summary>
    ///     倒计时器将以天、小时、分钟和秒的形式显示时间。
    /// </summary>
    Day,

    /// <summary>
    ///     倒计时器将以小时、分钟和秒的形式显示时间。
    /// </summary>
    Hour,

    /// <summary>
    ///     倒计时器将以秒的形式显示时间。
    /// </summary>
    Second
}
