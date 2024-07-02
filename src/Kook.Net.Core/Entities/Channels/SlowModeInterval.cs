namespace Kook;

/// <summary>
///     Specifies the slow-mode ratelimit in seconds for an <see cref="ITextChannel"/>.
///     表示 <see cref="T:Kook.ITextChannel"/> 的慢速模式延迟时间秒数。
/// </summary>
public enum SlowModeInterval
{
    /// <summary>
    ///     禁用慢速模式。
    /// </summary>
    None = 0,

    /// <summary>
    ///     每个用户每 5 秒最多发送一条消息。
    /// </summary>
    _5 = 5,

    /// <summary>
    ///     每个用户每 10 秒最多发送一条消息。
    /// </summary>
    _10 = 10,

    /// <summary>
    ///     每个用户每 15 秒最多发送一条消息。
    /// </summary>
    _15 = 15,

    /// <summary>
    ///     每个用户每 30 秒最多发送一条消息。
    /// </summary>
    _30 = 30,

    /// <summary>
    ///     每个用户每 1 分钟（60 秒）最多发送一条消息。
    /// </summary>
    _60 = 60,

    /// <summary>
    ///     每个用户每 2 分钟（120 秒）最多发送一条消息。
    /// </summary>
    _120 = 120,

    /// <summary>
    ///     每个用户每 5 分钟（300 秒）最多发送一条消息。
    /// </summary>
    _300 = 300,

    /// <summary>
    ///     每个用户每 10 分钟（600 秒）最多发送一条消息。
    /// </summary>
    _600 = 600,

    /// <summary>
    ///     每个用户每 15 分钟（900 秒）最多发送一条消息。
    /// </summary>
    _900 = 900,

    /// <summary>
    ///     每个用户每 30 分钟（1800 秒）最多发送一条消息。
    /// </summary>
    _1800 = 1800,

    /// <summary>
    ///     每个用户每 1 小时（3600 秒）最多发送一条消息。
    /// </summary>
    _3600 = 3600,

    /// <summary>
    ///     每个用户每 2 小时（7200 秒）最多发送一条消息。
    /// </summary>
    _7200 = 7200,

    /// <summary>
    ///     每个用户每 6 小时（21600 秒）最多发送一条消息。
    /// </summary>
    _21600 = 21600
}
