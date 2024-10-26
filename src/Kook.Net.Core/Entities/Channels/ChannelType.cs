namespace Kook;

/// <summary>
///     表示服务器频道的类型。
/// </summary>
public enum ChannelType
{
    /// <summary>
    ///     频道类型未指定。
    /// </summary>
    Unspecified = -1,

    /// <summary>
    ///     分组频道。
    /// </summary>
    Category = 0,

    /// <summary>
    ///     文字频道。
    /// </summary>
    Text = 1,

    /// <summary>
    ///     语音频道。
    /// </summary>
    Voice = 2,

    /// <summary>
    ///     私信频道。
    /// </summary>
    DM = 3,

    /// <summary>
    ///     帖子频道。
    /// </summary>
    Thread = 4
}
