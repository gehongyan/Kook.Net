namespace Kook;

/// <summary>
///     表示获取消息的方向。
/// </summary>
public enum Direction
{
    /// <summary>
    ///     未指定消息的获取方向。
    /// </summary>
    Unspecified,

    /// <summary>
    ///     以指定的参考消息为基准，向前获取消息。
    /// </summary>
    Before,

    /// <summary>
    ///     以指定的参考消息为基准，获取周围的消息。
    /// </summary>
    Around,

    /// <summary>
    ///     以指定的参考消息为基准，向后获取消息。
    /// </summary>
    After
}
