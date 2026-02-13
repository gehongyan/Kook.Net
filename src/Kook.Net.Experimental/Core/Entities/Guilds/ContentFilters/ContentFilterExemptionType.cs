namespace Kook;

/// <summary>
///     表示一个内容过滤器的豁免类型。
/// </summary>
public enum ContentFilterExemptionType
{
    /// <summary>
    ///     表示豁免一个文本频道。
    /// </summary>
    TextChannel = 1,

    /// <summary>
    ///     表示豁免一个类别频道。
    /// </summary>
    CategoryChannel = 2,

    /// <summary>
    ///     表示豁免一个服务器角色。
    /// </summary>
    Role = 3,
}