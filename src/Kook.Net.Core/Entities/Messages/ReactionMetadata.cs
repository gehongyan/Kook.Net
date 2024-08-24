namespace Kook;

/// <summary>
///     表示一个关于消息回应的元数据。
/// </summary>
public struct ReactionMetadata
{
    /// <summary>
    ///     获取此消息中已添加此回应的人数。
    /// </summary>
    public int ReactionCount { get; internal set; }

    /// <summary>
    ///     获取当前用户是否已对此消息做出回应。
    /// </summary>
    public bool IsMe { get; internal set; }
}
