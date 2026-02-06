using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个互动表情的随机结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record InteractiveEmoteRollResult
{
    internal InteractiveEmoteRollResult(int RawValue, string Image)
    {
        this.RawValue = RawValue;
        this.Image = Image;
    }

    private string DebuggerDisplay => $"Roll: {RawValue}";

    /// <summary> 获取随机结果的原始数值。 </summary>
    public int RawValue { get; }

    /// <summary> 获取随机结果对应的图片 URL。 </summary>
    public string Image { get; }

    /// <inheritdoc />
    public override string ToString() => RawValue.ToString();
}
