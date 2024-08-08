namespace Kook;

/// <summary>
///     表示一个消息的类型。
/// </summary>
public enum MessageType
{
    /// <summary>
    ///     纯文本消息。
    /// </summary>
    Text = 1,

    /// <summary>
    ///     图片消息。
    /// </summary>
    Image = 2,

    /// <summary>
    ///     视频消息。
    /// </summary>
    Video = 3,

    /// <summary>
    ///     文件消息。
    /// </summary>
    File = 4,

    /// <summary>
    ///     音频消息。
    /// </summary>
    Audio = 8,

    /// <summary>
    ///     KMarkdown 文本消息。
    /// </summary>
    KMarkdown = 9,

    /// <summary>
    ///     卡片消息。
    /// </summary>
    Card = 10,

    /// <summary>
    ///     POKE 消息。
    /// </summary>
    Poke = 12,

    /// <summary>
    ///     系统消息。
    /// </summary>
    System = 255
}
