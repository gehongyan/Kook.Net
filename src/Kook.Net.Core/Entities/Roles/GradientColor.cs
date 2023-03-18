using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a gradient color.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct GradientColor
{
    /// <summary>
    ///     Initializes a new instance of <see cref="GradientColor"/>.
    /// </summary>
    /// <param name="left"> The left color of the gradient. </param>
    /// <param name="right"> The right color of the gradient. </param>
    public GradientColor(Color left, Color right)
    {
        Left = left;
        Right = right;
    }

    /// <summary>
    ///     The left color of the gradient.
    /// </summary>
    public Color Left { get; }

    /// <summary>
    ///     The right color of the gradient.
    /// </summary>
    public Color Right { get; }

    /// <summary>
    ///     Converts the <see cref="GradientColor"/> to a tuple of <see cref="Color"/>.
    /// </summary>
    /// <param name="gradient"> The gradient color to convert. </param>
    /// <returns> The tuple of <see cref="Color"/>. </returns>
    public static implicit operator (Color Left, Color Right)(GradientColor gradient) => (gradient.Left, gradient.Right);
    /// <summary>
    ///     Converts the tuple of <see cref="Color"/> to a <see cref="GradientColor"/>.
    /// </summary>
    /// <param name="gradient"> The tuple of <see cref="Color"/> to convert. </param>
    /// <returns> The <see cref="GradientColor"/>. </returns>
    public static implicit operator GradientColor((Color Left, Color Right) gradient) => new(gradient.Left, gradient.Right);

    private string DebuggerDisplay => $"{Left} -> {Right}";
}
