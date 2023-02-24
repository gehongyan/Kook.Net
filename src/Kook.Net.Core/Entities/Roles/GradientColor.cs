using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a gradient color.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct GradientColor
{
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

    public static implicit operator (Color Left, Color Right)(GradientColor gradient) => (gradient.Left, gradient.Right);
    public static implicit operator GradientColor((Color Left, Color Right) gradient) => new(gradient.Left, gradient.Right);

    private string DebuggerDisplay => $"{Left} -> {Right}";
}
