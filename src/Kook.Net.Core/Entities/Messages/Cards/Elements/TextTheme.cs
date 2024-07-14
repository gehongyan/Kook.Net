namespace Kook;

/// <summary>
///     表示卡片内 KMarkdown 文本的颜色。
/// </summary>
/// <remarks>
///     <note type="warning">
///         彩色 KMarkdown 文本目前仅支持在卡片消息中使用，KMarkdown 文本消息暂不支持彩色。
///     </note>
///     <br />
///     <note type="warning">
///         彩色文本的颜色在不同的平台上可能会有所不同。建议您在不同的平台上测试您的卡片消息，以确保文本颜色符合您的预期。
///     </note>
/// </remarks>
/// <seealso cref="M:Kook.Format.Colorize(System.String,Kook.TextTheme,System.Boolean)"/>。
public enum TextTheme : ushort
{
    /// <summary>
    ///     外观表现为主要文本。
    /// </summary>
    /// <remarks>
    ///     按钮的前景色大体为草绿色，浅色模式下为 <c>#7ACC35</c>（http://www.color-hex.com/color/7acc35），深色模式下为
    ///     <c>#6CBF00</c>（http://www.color-hex.com/color/6cbf00）。
    /// </remarks>
    Primary,

    /// <summary>
    ///     外观表现为成功。
    /// </summary>
    /// <remarks>
    ///     文本的前景色大体为薄荷绿色 <c>#00D287</c>（http://www.color-hex.com/color/00d287）。
    /// </remarks>
    Success,

    /// <summary>
    ///     外观表现为危险。
    /// </summary>
    /// <remarks>
    ///     文本的前景色大体为鲜红色，浅色模式下为 <c>#FF3200</c>（http://www.color-hex.com/color/ff3200），深色模式下为
    ///     <c>#FF4D42</c>（http://www.color-hex.com/color/ff4d42）。
    /// </remarks>
    Danger,

    /// <summary>
    ///     外观表现为警告。
    /// </summary>
    /// <remarks>
    ///     文本的前景色大体为橙色，浅色模式下为 <c>#FF8200</c>（http://www.color-hex.com/color/ff8200），深色模式下为
    ///     <c>#FF8F19</c>（http://www.color-hex.com/color/ff8f19）。
    /// </remarks>
    Warning,

    /// <summary>
    ///     外观表现为信息。
    /// </summary>
    /// <remarks>
    ///     此颜色主题可能不生效。
    /// </remarks>
    Info,

    /// <summary>
    ///     外观表现为次要文本。
    /// </summary>
    /// <remarks>
    ///     文本的前景色大体为灰色，浅色模式下为 <c>#666666</c>（http://www.color-hex.com/color/666666），深色模式下为
    ///     <c>#AAAAAA</c>（http://www.color-hex.com/color/aaaaaa）。
    /// </remarks>
    Secondary,

    /// <summary>
    ///     外观表现为正文。
    /// </summary>
    /// <remarks>
    ///     文本的前景色大体为黑色或白色，浅色模式下为 <c>#222222</c>（http://www.color-hex.com/color/222222），深色模式下为
    ///     <c>#FFFFFF</c>（http://www.color-hex.com/color/ffffff）。
    /// </remarks>
    Body,

    /// <summary>
    ///     外观表现为提示。
    /// </summary>
    /// <remarks>
    ///     文本的前景色大体为与背景色差别较小的灰色，浅色模式下为 <c>#999999</c>（http://www.color-hex.com/color/999999），深色模式下为
    ///     <c>#777777</c>（http://www.color-hex.com/color/777777）。
    /// </remarks>
    Tips,

    /// <summary>
    ///     外观表现为粉色。
    /// </summary>
    /// <remarks>
    ///     文本的前景色大体为粉色，浅色模式下为 <c>#F65299</c>（http://www.color-hex.com/color/f65299），深色模式下为
    ///     <c>#F765A5</c>（http://www.color-hex.com/color/F765A5）。
    /// </remarks>
    Pink,

    /// <summary>
    ///     外观表现为紫色。
    /// </summary>
    /// <remarks>
    ///     文本的前景色大体为紫色，浅色模式下为 <c>#853EFD</c>（http://www.color-hex.com/color/853efd），深色模式下为
    ///     <c>#9557FE</c>（http://www.color-hex.com/color/9557fe）。
    /// </remarks>
    Purple
}


