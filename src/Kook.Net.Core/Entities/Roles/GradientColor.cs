using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     表示一个渐变色。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct GradientColor : IEquatable<GradientColor>
{
    /// <summary>
    ///     初始化一个 <see cref="GradientColor"/> 结构的新实例。
    /// </summary>
    /// <param name="left"> 渐变色的左侧颜色。 </param>
    /// <param name="right"> 渐变色的右侧颜色。 </param>
    public GradientColor(Color left, Color right)
    {
        Left = left;
        Right = right;
    }

    /// <summary>
    ///     获取此渐变色的左侧颜色。
    /// </summary>
    public Color Left { get; }

    /// <summary>
    ///     获取此渐变色的右侧颜色。
    /// </summary>
    public Color Right { get; }

    /// <summary>
    ///     将此渐变色解构为表示左右两个颜色的元组。
    /// </summary>
    /// <param name="left"> 左侧颜色。 </param>
    /// <param name="right"> 右侧颜色。 </param>
    public void Deconstruct(out Color left, out Color right)
    {
        left = Left;
        right = Right;
    }

    /// <summary>
    ///     将此渐变色转换为表示左右两个颜色的元组。
    /// </summary>
    /// <param name="gradient"> 要转换的渐变色。 </param>
    /// <returns> 表示左右两个颜色的元组。 </returns>
    public static implicit operator (Color Left, Color Right)(GradientColor gradient) => (gradient.Left, gradient.Right);

    /// <summary>
    ///     将此表示左右两个颜色的元组转换为渐变色。
    /// </summary>
    /// <param name="gradient"> 要转换的表示左右两个颜色的元组。 </param>
    /// <returns> 转换后的渐变色。 </returns>
    public static implicit operator GradientColor((Color Left, Color Right) gradient) => new(gradient.Left, gradient.Right);

    private string DebuggerDisplay => $"{Left} -> {Right}";

    /// <inheritdoc />
    public bool Equals(GradientColor other) => Left.Equals(other.Left) && Right.Equals(other.Right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is GradientColor other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Left.GetHashCode() * 397) ^ Right.GetHashCode();
        }
    }
}
