namespace Kook;

/// <summary>
///     表示 <see cref="ButtonElement"/> 的主题。
/// </summary>
public enum ButtonTheme
{
    /// <summary>
    ///     外观表现为主要操作。
    /// </summary>
    /// <remarks>
    ///     按钮的背景色大体为草绿色，浅色模式下为 <c>#7ACC35</c>（http://www.color-hex.com/color/7acc35），深色模式下为
    ///     <c>#6CBF00</c>（http://www.color-hex.com/color/6cbf00）。
    /// </remarks>
    Primary,

    /// <summary>
    ///     外观表现为成功。
    /// </summary>
    /// <remarks>
    ///     按钮的背景色大体为薄荷绿色 <c>#00D287</c>（http://www.color-hex.com/color/00d287）。
    /// </remarks>
    Success,

    /// <summary>
    ///     外观表现为警告。
    /// </summary>
    /// <remarks>
    ///     按钮的背景色大体为橙色，浅色模式下为 <c>#FF8200</c>（http://www.color-hex.com/color/ff8200），深色模式下为
    ///     <c>#FF8F19</c>（http://www.color-hex.com/color/ff8f19）。
    /// </remarks>
    Warning,

    /// <summary>
    ///     外观表现为危险。
    /// </summary>
    /// <remarks>
    ///     按钮的背景色大体为鲜红色，浅色模式下为 <c>#FF3200</c>（http://www.color-hex.com/color/ff3200），深色模式下为
    ///     <c>#FF4D42</c>（http://www.color-hex.com/color/ff4d42）。
    /// </remarks>
    Danger,

    /// <summary>
    ///     外观表现为信息。
    /// </summary>
    /// <remarks>
    ///     按钮的背景色大体为天蓝色，浅色模式下为 <c>#0096FF</c>（http://www.color-hex.com/color/0096ff），深色模式下为
    ///     <c>#33AAFF</c>（http://www.color-hex.com/color/33aaff）。
    /// </remarks>
    Info,

    /// <summary>
    ///     外观表现为次要操作。
    /// </summary>
    /// <remarks>
    ///     按钮的背景色大体为灰色 <c>#BBBEC4</c>（http://www.color-hex.com/color/bbbec4），不透明度为 <c>0.3</c>。
    /// </remarks>
    Secondary
}
