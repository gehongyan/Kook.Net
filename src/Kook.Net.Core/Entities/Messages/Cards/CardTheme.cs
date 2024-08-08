namespace Kook;

/// <summary>
///     表示卡片的主题。
/// </summary>
/// <remarks>
///     卡片的主题主要用于控制卡片左侧边的颜色。
/// </remarks>
public enum CardTheme : uint
{
    /// <summary>
    ///     外观表现为主要卡片。
    /// </summary>
    /// <remarks>
    ///     此主题与 <see cref="Info"/> 主题相同。 <br />
    ///     卡片侧边的颜色大体为天蓝色，浅色模式下为 <c>#0096FF</c>（http://www.color-hex.com/color/0096ff），深色模式下为
    ///     <c>#33AAFF</c>（http://www.color-hex.com/color/33aaff）。
    /// </remarks>
    Primary,

    /// <summary>
    ///     外观表现为成功。
    /// </summary>
    /// <remarks>
    ///     卡片侧边的颜色大体为薄荷绿色 <c>#00D287</c>（http://www.color-hex.com/color/00d287）。
    /// </remarks>
    Success,

    /// <summary>
    ///     外观表现为警告。
    /// </summary>
    /// <remarks>
    ///     卡片侧边的颜色大体为橙色，浅色模式下为 <c>#FF8200</c>（http://www.color-hex.com/color/ff8200），深色模式下为
    ///     <c>#FF8F19</c>（http://www.color-hex.com/color/ff8f19）。
    /// </remarks>
    Warning,

    /// <summary>
    ///     外观表现为危险。
    /// </summary>
    /// <remarks>
    ///     卡片侧边的颜色大体为鲜红色，浅色模式下为 <c>#FF3200</c>（http://www.color-hex.com/color/ff3200），深色模式下为
    ///     <c>#FF4D42</c>（http://www.color-hex.com/color/ff4d42）。
    /// </remarks>
    Danger,

    /// <summary>
    ///     外观表现为信息。
    /// </summary>
    /// <remarks>
    ///     此主题与 <see cref="Primary"/> 主题相同。 <br />
    ///     卡片侧边的颜色大体为天蓝色，浅色模式下为 <c>#0096FF</c>（http://www.color-hex.com/color/0096ff），深色模式下为
    ///     <c>#33AAFF</c>（http://www.color-hex.com/color/33aaff）。
    /// </remarks>
    Info,

    /// <summary>
    ///     外观表现为次要卡片。
    /// </summary>
    /// <remarks>
    ///     卡片侧边的颜色大体为灰色，浅色模式下为 <c>#666666</c>（http://www.color-hex.com/color/666666），深色模式下为
    ///     <c>#AAAAAA</c>（http://www.color-hex.com/color/aaaaaa）。
    /// </remarks>
    Secondary,

    /// <summary>
    ///     外观表现为无侧边。
    /// </summary>
    /// <remarks>
    ///     卡片无侧边。
    /// </remarks>
    None,

    /// <summary>
    ///     外观表现为图文混排消息。
    /// </summary>
    /// <remarks>
    ///     卡片无侧边，无底色，无边框。此主题用于图文混排消息的展示。
    /// </remarks>
    Invisible
}
