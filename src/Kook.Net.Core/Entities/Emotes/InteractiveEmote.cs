using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个互动表情符号。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class InteractiveEmote : Emote
{
    /// <summary>
    ///     创建一个新的 <see cref="Kook.InteractiveEmote" /> 实例。
    /// </summary>
    internal InteractiveEmote(string id, string name, InteractiveEmoteType interactiveType, string? dynamicImage)
        : base(id, name, EmojiType.Interactive)
    {
        DynamicImage = dynamicImage;
        InteractiveEmoteType = interactiveType;
    }

    /// <summary>
    ///     获取此互动表情符号的类型。
    /// </summary>
    public InteractiveEmoteType InteractiveEmoteType { get; }

    /// <summary>
    ///     获取此互动表情符号在显示随机结果前的动画资源图像的 URL。
    /// </summary>
    /// <remarks>
    ///     如果此动态表情符号不是从 KOOK API 或网关生成的，则此属性为 <c>null</c>。
    /// </remarks>
    public string? DynamicImage { get; internal set; }

    private string DebuggerDisplay => $"{Name} ({Id}{(Type.HasValue ? $", {Type}" : "")})";
}
