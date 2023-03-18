namespace Kook;

/// <summary>
///     Specifies the slow-mode ratelimit in seconds for an <see cref="ITextChannel"/>.
/// </summary>
public enum SlowModeInterval
{
    /// <summary>
    ///     Slow-mode is disabled.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Each user needs to wait for 5 seconds before sending another message.
    /// </summary>
    _5 = 5,

    /// <summary>
    ///     Each user needs to wait for 10 seconds before sending another message.
    /// </summary>
    _10 = 10,

    /// <summary>
    ///     Each user needs to wait for 15 seconds before sending another message.
    /// </summary>
    _15 = 15,

    /// <summary>
    ///     Each user needs to wait for 30 seconds before sending another message.
    /// </summary>
    _30 = 30,

    /// <summary>
    ///     Each user needs to wait for 1 minute (60 seconds) before sending another message.
    /// </summary>
    _60 = 60,

    /// <summary>
    ///     Each user needs to wait for 2 minutes (120 seconds) before sending another message.
    /// </summary>
    _120 = 120,

    /// <summary>
    ///     Each user needs to wait for 5 minutes (300 seconds) before sending another message.
    /// </summary>
    _300 = 300,

    /// <summary>
    ///     Each user needs to wait for 10 minutes (600 seconds) before sending another message.
    /// </summary>
    _600 = 600,

    /// <summary>
    ///     Each user needs to wait for 15 minutes (900 seconds) before sending another message.
    /// </summary>
    _900 = 900,

    /// <summary>
    ///     Each user needs to wait for 30 minutes (1800 seconds) before sending another message.
    /// </summary>
    _1800 = 1800,

    /// <summary>
    ///     Each user needs to wait for 1 hour (3600 seconds) before sending another message.
    /// </summary>
    _3600 = 3600,

    /// <summary>
    ///     Each user needs to wait for 2 hours (7200 seconds) before sending another message.
    /// </summary>
    _7200 = 7200,

    /// <summary>
    ///     Each user needs to wait for 6 hours (21600 seconds) before sending another message.
    /// </summary>
    _21600 = 21600
}
