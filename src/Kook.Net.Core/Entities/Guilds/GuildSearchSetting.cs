namespace Kook;

/// <summary>
///     表示服务器的搜索设置。
/// </summary>
public enum GuildSearchSetting
{
    /// <summary>
    ///     不公开，不允许通过ID或关键词搜索加入
    /// </summary>
    Private = 1,

    /// <summary>
    ///     仅允许通过 ID 搜索与加入
    /// </summary>
    IdOnly = 2,

    /// <summary>
    ///     允许通过 ID 或关键词搜索与加入
    /// </summary>
    Public = 3
}
