using System.Diagnostics;
using StandardColor = System.Drawing.Color;

namespace Kook;

/// <summary>
///     Represents a color used in Kook.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct Color
{
    /// <summary> Gets the max decimal value of color. </summary>
    public const uint MaxDecimalValue = 0xFFFFFF;
    /// <summary> Gets the default user color value. </summary>
    public static readonly Color Default = new(0);
    /// <summary> Gets the teal color value. </summary>
    /// <returns> <para>A color struct with the hex value of 1ABC9C.</para> See http://www.color-hex.com/color/1ABC9C</returns>
    public static readonly Color Teal = new(0x1ABC9C);
    /// <summary> Gets the dark teal color value. </summary>
    /// <returns> <para>A color struct with the hex value of 11806A.</para> See http://www.color-hex.com/color/11806A</returns>
    public static readonly Color DarkTeal = new(0x11806A);
    /// <summary> Gets the green color value. </summary>
    /// <returns> <para>A color struct with the hex value of 2ECC71.</para> See http://www.color-hex.com/color/2ECC71</returns>
    public static readonly Color Green = new(0x2ECC71);
    /// <summary> Gets the dark green color value. </summary>
    /// <returns> <para>A color struct with the hex value of 1F8B4C.</para> See http://www.color-hex.com/color/1F8B4C</returns>
    public static readonly Color DarkGreen = new(0x1F8B4C);
    /// <summary> Gets the blue color value. </summary>
    /// <returns> <para>A color struct with the hex value of 3498DB.</para> See http://www.color-hex.com/color/3498DB</returns>
    public static readonly Color Blue = new(0x3498DB);
    /// <summary> Gets the dark blue color value. </summary>
    /// <returns> <para>A color struct with the hex value of 206694.</para> See http://www.color-hex.com/color/206694</returns>
    public static readonly Color DarkBlue = new(0x206694);
    /// <summary> Gets the purple color value. </summary>
    /// <returns> <para>A color struct with the hex value of 9B59B6.</para> See http://www.color-hex.com/color/9B59B6</returns>
    public static readonly Color Purple = new(0x9B59B6);
    /// <summary> Gets the dark purple color value. </summary>
    /// <returns> <para>A color struct with the hex value of 71368A.</para> See http://www.color-hex.com/color/71368A</returns>
    public static readonly Color DarkPurple = new(0x71368A);
    /// <summary> Gets the magenta color value. </summary>
    /// <returns> <para>A color struct with the hex value of E91E63.</para> See http://www.color-hex.com/color/E91E63</returns>
    public static readonly Color Magenta = new(0xE91E63);
    /// <summary> Gets the dark magenta color value. </summary>
    /// <returns> <para>A color struct with the hex value of AD1457.</para> See http://www.color-hex.com/color/AD1457</returns>
    public static readonly Color DarkMagenta = new(0xAD1457);
    /// <summary> Gets the gold color value. </summary>
    /// <returns> <para>A color struct with the hex value of F1C40F.</para> See http://www.color-hex.com/color/F1C40F</returns>
    public static readonly Color Gold = new(0xF1C40F);
    /// <summary> Gets the light orange color value. </summary>
    /// <returns> <para>A color struct with the hex value of C27C0E.</para> See http://www.color-hex.com/color/C27C0E</returns>
    public static readonly Color LightOrange = new(0xC27C0E);
    /// <summary> Gets the orange color value. </summary>
    /// <returns> <para>A color struct with the hex value of E67E22.</para> See http://www.color-hex.com/color/E67E22</returns>
    public static readonly Color Orange = new(0xE67E22);
    /// <summary> Gets the dark orange color value. </summary>
    /// <returns> <para>A color struct with the hex value of A84300.</para> See http://www.color-hex.com/color/A84300</returns>
    public static readonly Color DarkOrange = new(0xA84300);
    /// <summary> Gets the red color value. </summary>
    /// <returns> <para>A color struct with the hex value of E74C3C.</para> See http://www.color-hex.com/color/E74C3C</returns>
    public static readonly Color Red = new(0xE74C3C);
    /// <summary> Gets the dark red color value. </summary>
    /// <returns> <para>A color struct with the hex value of 992D22.</para> See http://www.color-hex.com/color/992D22</returns>
    public static readonly Color DarkRed = new(0x992D22);
    /// <summary> Gets the light grey color value. </summary>
    /// <returns> <para>A color struct with the hex value of 95A5A6.</para> See http://www.color-hex.com/color/95A5A6</returns>
    public static readonly Color LightGrey = new(0x95A5A6);
    /// <summary> Gets the grey color value. </summary>
    /// <returns> <para>A color struct with the hex value of 666D71.</para> See http://www.color-hex.com/color/666D71</returns>
    public static readonly Color Grey = new(0x666D71);
    /// <summary> Gets the dark grey color value. </summary>
    /// <returns> <para>A color struct with the hex value of 607D8B.</para> See http://www.color-hex.com/color/607D8B</returns>
    public static readonly Color DarkGrey = new(0x607D8B);
    /// <summary> Gets the darker grey color value. </summary>
    /// <returns> <para>A color struct with the hex value of 3A4B53.</para> See http://www.color-hex.com/color/3A4B53</returns>
    public static readonly Color DarkerGrey = new(0x3A4B53);

    /// <summary> Gets the encoded value for this color. </summary>
    /// <remarks>
    ///     This value is encoded as an unsigned integer value. The most-significant 8 bits contain the red value,
    ///     the middle 8 bits contain the green value, and the least-significant 8 bits contain the blue value.
    /// </remarks>
    public uint RawValue { get; }

    /// <summary> Gets the red component for this color. </summary>
    public byte R => (byte)(RawValue >> 16);
    /// <summary> Gets the green component for this color. </summary>
    public byte G => (byte)(RawValue >> 8);
    /// <summary> Gets the blue component for this color. </summary>
    public byte B => (byte)(RawValue);

    /// <summary>
    ///     Initializes a <see cref="Color"/> struct with the given raw value.
    /// </summary>
    /// <example>
    ///     The following will create a color that has a hex value of
    ///     <see href="http://www.color-hex.com/color/607d8b">#607D8B</see>.
    ///     <code language="cs">
    ///     Color darkGrey = new Color(0x607D8B);
    ///     </code>
    /// </example>
    /// <param name="rawValue">The raw value of the color (e.g. <c>0x607D8B</c>).</param>
    /// <exception cref="ArgumentException">Value exceeds <see cref="MaxDecimalValue"/>.</exception>
    public Color(uint rawValue)
    {
        if (rawValue > MaxDecimalValue)
            throw new ArgumentException($"{nameof(RawValue)} of color cannot be greater than {MaxDecimalValue}!", nameof(rawValue));

        RawValue = rawValue;
    }

    /// <summary>
    ///     Initializes a <see cref="Color" /> struct with the given RGB bytes.
    /// </summary>
    /// <example>
    ///     The following will create a color that has a value of
    ///     <see href="http://www.color-hex.com/color/607d8b">#607D8B</see>.
    ///     <code language="cs">
    ///     Color darkGrey = new Color((byte)0b_01100000, (byte)0b_01111101, (byte)0b_10001011);
    ///     </code>
    /// </example>
    /// <param name="r">The byte that represents the red color.</param>
    /// <param name="g">The byte that represents the green color.</param>
    /// <param name="b">The byte that represents the blue color.</param>
    /// <exception cref="ArgumentException">Value exceeds <see cref="MaxDecimalValue"/>.</exception>
    public Color(byte r, byte g, byte b)
    {
        uint value = ((uint)r << 16)
                     | ((uint)g << 8)
                     | (uint)b;

        if (value > MaxDecimalValue)
            throw new ArgumentException($"{nameof(RawValue)} of color cannot be greater than {MaxDecimalValue}!");

        RawValue = value;
    }

    /// <summary>
    ///     Initializes a <see cref="Color"/> struct with the given RGB value.
    /// </summary>
    /// <example>
    ///     The following will create a color that has a value of
    ///     <see href="http://www.color-hex.com/color/607d8b">#607D8B</see>.
    ///     <code language="cs">
    ///     Color darkGrey = new Color(96, 125, 139);
    ///     </code>
    /// </example>
    /// <param name="r">The value that represents the red color. Must be within 0~255.</param>
    /// <param name="g">The value that represents the green color. Must be within 0~255.</param>
    /// <param name="b">The value that represents the blue color. Must be within 0~255.</param>
    /// <exception cref="ArgumentOutOfRangeException">The argument value is not between 0 to 255.</exception>
    public Color(int r, int g, int b)
    {
        if (r < 0 || r > 255)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,255].");
        if (g < 0 || g > 255)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,255].");
        if (b < 0 || b > 255)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,255].");
        RawValue = ((uint)r << 16)
                   | ((uint)g << 8)
                   | (uint)b;
    }
    /// <summary>
    ///     Initializes a <see cref="Color"/> struct with the given RGB float value.
    /// </summary>
    /// <example>
    ///     The following will create a color that has a value of
    ///     <see href="http://www.color-hex.com/color/607c8c">#607c8c</see>.
    ///     <code language="cs">
    ///     Color darkGrey = new Color(0.38f, 0.49f, 0.55f);
    ///     </code>
    /// </example>
    /// <param name="r">The value that represents the red color. Must be within 0~1.</param>
    /// <param name="g">The value that represents the green color. Must be within 0~1.</param>
    /// <param name="b">The value that represents the blue color. Must be within 0~1.</param>
    /// <exception cref="ArgumentOutOfRangeException">The argument value is not between 0 to 1.</exception>
    public Color(float r, float g, float b)
    {
        if (r < 0.0f || r > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,1].");
        if (g < 0.0f || g > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,1].");
        if (b < 0.0f || b > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,1].");
        RawValue = ((uint)(r * 255.0f) << 16)
                   | ((uint)(g * 255.0f) << 8)
                   | (uint)(b * 255.0f);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Color" /> is equal to this instance.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="Color" /> is equal to this instance; otherwise, <c>false</c> . </returns>
    public static bool operator ==(Color lhs, Color rhs)
        => lhs.RawValue == rhs.RawValue;

    /// <summary>
    ///     Determines whether the specified <see cref="Color" /> is not equal to this instance.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="Color" /> is not equal to this instance; otherwise, <c>false</c> . </returns>
    public static bool operator !=(Color lhs, Color rhs)
        => lhs.RawValue != rhs.RawValue;

    /// <summary>
    ///     Converts the given raw value of <see cref="uint"/> to a <see cref="Color"/>.
    /// </summary>
    /// <param name="rawValue"> The raw value of the color. </param>
    /// <returns> The <see cref="Color"/> that represents the given raw value. </returns>
    public static implicit operator Color(uint rawValue)
        => new(rawValue);

    /// <summary>
    ///     Converts the given <see cref="Color"/> to its raw value of <see cref="uint"/>.
    /// </summary>
    /// <param name="color"> The <see cref="Color"/> to convert. </param>
    /// <returns> The raw value of the given <see cref="Color"/>. </returns>
    public static implicit operator uint(Color color)
        => color.RawValue;

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is Color c && RawValue == c.RawValue;

    /// <inheritdoc />
    public override int GetHashCode() => RawValue.GetHashCode();

    /// <summary>
    ///     Converts the given Kook.Net-defined <see cref="Color"/> to a .NET standard <see cref="StandardColor"/>.
    /// </summary>
    /// <param name="color"> The Kook.Net-defined <see cref="Color"/> to convert. </param>
    /// <returns> The .NET standard <see cref="StandardColor"/> that represents the given Kook.Net-defined <see cref="Color"/>. </returns>
    public static implicit operator StandardColor(Color color)
        => StandardColor.FromArgb((int)color.RawValue);

    /// <summary>
    ///     Converts the given .NET standard <see cref="StandardColor"/> to a Kook.Net-defined <see cref="Color"/>.
    /// </summary>
    /// <param name="color"> The .NET standard <see cref="StandardColor"/> to convert. </param>
    /// <returns> The Kook.Net-defined <see cref="Color"/> that represents the given .NET standard <see cref="StandardColor"/>. </returns>
    public static explicit operator Color(StandardColor color)
        => new((uint)color.ToArgb() << 8 >> 8);

    /// <summary>
    ///     Gets the hexadecimal representation of the color (e.g. <c>#000ccc</c>).
    /// </summary>
    /// <returns>
    ///     A hexadecimal string of the color.
    /// </returns>
    public override string ToString() => $"#{RawValue:X6}";

    private string DebuggerDisplay => $"#{RawValue:X6} ({RawValue})";
}
