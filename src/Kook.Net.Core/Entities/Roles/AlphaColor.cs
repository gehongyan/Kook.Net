using System.Diagnostics;
using StandardColor = System.Drawing.Color;

namespace Kook;

/// <summary>
///     Represents a <see cref="Color"/> with an alpha channel.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct AlphaColor
{
    /// <summary> Gets the max decimal value of an color with an alpha channel. </summary>
    public const ulong MaxDecimalValue = 0xFFFFFFFF;

    /// <summary> Gets the default user color value. </summary>
    public static readonly AlphaColor Default = new(Color.Default, 0xFF);

    /// <summary>
    ///     Gets the raw value for this color.
    /// </summary>
    public ulong RawValue { get; }

    /// <summary> Gets the red component for this color. </summary>
    public byte R => (byte)(RawValue >> 24);

    /// <summary> Gets the green component for this color. </summary>
    public byte G => (byte)(RawValue >> 16);

    /// <summary> Gets the blue component for this color. </summary>
    public byte B => (byte)(RawValue >> 8);

    /// <summary> Gets the alpha component for this color. </summary>
    public byte A => (byte)RawValue;

    /// <summary>
    ///     Gets the base color for this color without the alpha channel.
    /// </summary>
    public Color BaseColor => new((uint)RawValue >> 8);

    /// <summary>
    ///     Initializes a new instance of the <see cref="AlphaColor"/> struct with the specified raw value.
    /// </summary>
    /// <param name="rawValue"> The raw value to use. </param>
    public AlphaColor(ulong rawValue)
    {
        if (rawValue > MaxDecimalValue)
            throw new ArgumentException($"{nameof(RawValue)} of color cannot be greater than {MaxDecimalValue}!", nameof(rawValue));

        RawValue = rawValue;
    }

    /// <summary>
    ///     Initializes a <see cref="AlphaColor" /> struct with the given base color and alpha channel.
    /// </summary>
    /// <param name="baseColor"> The base color to use. </param>
    /// <param name="alpha">The byte that represents the alpha channel.</param>
    /// <exception cref="ArgumentException">Value exceeds <see cref="MaxDecimalValue"/>.</exception>
    public AlphaColor(Color baseColor, byte alpha)
    {
        ulong value = ((ulong)baseColor.R << 24)
            | ((ulong)baseColor.G << 16)
            | ((ulong)baseColor.B << 8)
            | (ulong)alpha;

        if (value > MaxDecimalValue)
            throw new ArgumentException($"{nameof(RawValue)} of color cannot be greater than {MaxDecimalValue}!");

        RawValue = value;
    }

    /// <summary>
    ///     Initializes a <see cref="AlphaColor" /> struct with the given RGBA bytes.
    /// </summary>
    /// <example>
    ///     The following will create a color that has a value of <c>#607D8BFF</c>.
    ///     <code language="cs">
    ///     AlphaColor darkGrey = new AlphaColor((byte)0b_01100000, (byte)0b_01111101, (byte)0b_10001011, (byte)0b_11111111);
    ///     </code>
    /// </example>
    /// <param name="r">The byte that represents the red color.</param>
    /// <param name="g">The byte that represents the green color.</param>
    /// <param name="b">The byte that represents the blue color.</param>
    /// <param name="a">The byte that represents the alpha channel.</param>
    /// <exception cref="ArgumentException">Value exceeds <see cref="MaxDecimalValue"/>.</exception>
    public AlphaColor(byte r, byte g, byte b, byte a)
    {
        ulong value = ((ulong)r << 24)
            | ((ulong)g << 16)
            | ((ulong)b << 8)
            | (ulong)a;

        if (value > MaxDecimalValue)
            throw new ArgumentException($"{nameof(RawValue)} of color cannot be greater than {MaxDecimalValue}!");

        RawValue = value;
    }

    /// <summary>
    ///     Initializes a <see cref="AlphaColor"/> struct with the given RGBA value.
    /// </summary>
    /// <example>
    ///     The following will create a color that has a value of <c>#607D8BFF</c>.
    ///     <code language="cs">
    ///     AlphaColor darkGrey = new AlphaColor(96, 125, 139, 255);
    ///     </code>
    /// </example>
    /// <param name="r">The value that represents the red color. Must be within 0~255.</param>
    /// <param name="g">The value that represents the green color. Must be within 0~255.</param>
    /// <param name="b">The value that represents the blue color. Must be within 0~255.</param>
    /// <param name="a">The value that represents the alpha channel. Must be within 0~255.</param>
    /// <exception cref="ArgumentOutOfRangeException">The argument value is not between 0 to 255.</exception>
    public AlphaColor(int r, int g, int b, int a)
    {
        if (r < 0 || r > 255)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,255].");

        if (g < 0 || g > 255)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,255].");

        if (b < 0 || b > 255)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,255].");

        if (a < 0 || a > 255)
            throw new ArgumentOutOfRangeException(nameof(a), "Value must be within [0,255].");

        RawValue = ((ulong)(uint)r << 24)
            | ((ulong)(uint)g << 16)
            | ((ulong)(uint)b << 8)
            | (ulong)(uint)a;
    }


    /// <summary>
    ///     Initializes a <see cref="AlphaColor"/> struct with the given RGBA float value.
    /// </summary>
    /// <example>
    ///     The following will create a color that has a value of <c>#607C8CFF</c>.
    ///     <code language="cs">
    ///     AlphaColor darkGrey = new AlphaColor(0.38f, 0.49f, 0.55f, 1.00f);
    ///     </code>
    /// </example>
    /// <param name="r">The value that represents the red color. Must be within 0~1.</param>
    /// <param name="g">The value that represents the green color. Must be within 0~1.</param>
    /// <param name="b">The value that represents the blue color. Must be within 0~1.</param>
    /// <param name="a">The value that represents the alpha channel. Must be within 0~1.</param>
    /// <exception cref="ArgumentOutOfRangeException">The argument value is not between 0 to 1.</exception>
    public AlphaColor(float r, float g, float b, float a)
    {
        if (r < 0.0f || r > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,1].");

        if (g < 0.0f || g > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,1].");

        if (b < 0.0f || b > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,1].");

        if (a < 0.0f || a > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(a), "Value must be within [0,1].");

        RawValue = ((uint)(r * 255.0f) << 24)
            | ((uint)(g * 255.0f) << 16)
            | ((uint)(b * 255.0f) << 8)
            | (uint)(a * 255.0f);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="AlphaColor" /> is equal to this instance.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="AlphaColor" /> is equal to this instance; otherwise, <c>false</c> . </returns>
    public static bool operator ==(AlphaColor lhs, AlphaColor rhs)
        => lhs.RawValue == rhs.RawValue;

    /// <summary>
    ///     Determines whether the specified <see cref="AlphaColor" /> is not equal to this instance.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="AlphaColor" /> is not equal to this instance; otherwise, <c>false</c> . </returns>
    public static bool operator !=(AlphaColor lhs, AlphaColor rhs)
        => lhs.RawValue != rhs.RawValue;

    /// <summary>
    ///     Converts the given raw value of <see cref="uint"/> to a <see cref="AlphaColor"/>.
    /// </summary>
    /// <param name="rawValue"> The raw value of the color. </param>
    /// <returns> The <see cref="AlphaColor"/> that represents the given raw value. </returns>
    public static implicit operator AlphaColor(ulong rawValue)
        => new(rawValue);

    /// <summary>
    ///     Converts the given <see cref="AlphaColor"/> to its raw value of <see cref="uint"/>.
    /// </summary>
    /// <param name="color"> The <see cref="AlphaColor"/> to convert. </param>
    /// <returns> The raw value of the given <see cref="AlphaColor"/>. </returns>
    public static implicit operator ulong(AlphaColor color)
        => color.RawValue;

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is AlphaColor c && RawValue == c.RawValue;

    /// <inheritdoc />
    public override int GetHashCode() => RawValue.GetHashCode();

    /// <summary>
    ///     Converts the given Kook.Net-defined <see cref="Color"/> to a Kook.Net-defined <see cref="AlphaColor"/>.
    /// </summary>
    /// <param name="color"> The Kook.Net-defined <see cref="Color"/> to convert. </param>
    /// <returns> The Kook.Net-defined <see cref="AlphaColor"/> that represents the given Kook.Net-defined <see cref="Color"/>. </returns>
    public static implicit operator AlphaColor(Color color)
        => new(((ulong)color.RawValue << 8) | 0xFF);

    /// <summary>
    ///     Converts the given Kook.Net-defined <see cref="AlphaColor"/> to a Kook.Net-defined <see cref="Color"/>.
    /// </summary>
    /// <param name="color"> The Kook.Net-defined <see cref="AlphaColor"/> to convert. </param>
    /// <returns> The Kook.Net-defined <see cref="Color"/> that represents the given Kook.Net-defined <see cref="AlphaColor"/>. </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This conversion will drop the alpha channel of the given <see cref="AlphaColor"/>.
    ///     </note>
    /// </remarks>
    public static explicit operator Color(AlphaColor color) => color.BaseColor;

    /// <summary>
    ///     Converts the given Kook.Net-defined <see cref="AlphaColor"/> to a .NET standard <see cref="StandardColor"/>.
    /// </summary>
    /// <param name="color"> The Kook.Net-defined <see cref="AlphaColor"/> to convert. </param>
    /// <returns> The .NET standard <see cref="StandardColor"/> that represents the given Kook.Net-defined <see cref="AlphaColor"/>. </returns>
    public static implicit operator StandardColor(AlphaColor color)
        => StandardColor.FromArgb(color.A, color.R, color.G, color.B);

    /// <summary>
    ///     Converts the given .NET standard <see cref="StandardColor"/> to a Kook.Net-defined <see cref="AlphaColor"/>.
    /// </summary>
    /// <param name="color"> The .NET standard <see cref="StandardColor"/> to convert. </param>
    /// <returns> The Kook.Net-defined <see cref="AlphaColor"/> that represents the given .NET standard <see cref="StandardColor"/>. </returns>
    public static explicit operator AlphaColor(StandardColor color)
        => new(color.R, color.G, color.B, color.A);

    /// <summary>
    ///     Gets the hexadecimal representation of the color (e.g. <c>#000cccff</c>).
    /// </summary>
    /// <returns>
    ///     A hexadecimal string of the color.
    /// </returns>
    public override string ToString() => $"#{RawValue:X8}";

    private string DebuggerDisplay => $"#{RawValue:X8} ({RawValue})";
}
