using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using StandardColor = System.Drawing.Color;

namespace Kook;

/// <summary>
///     表示 KOOK 中使用的颜色。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct Color
{
    /// <summary>
    ///     获取一个 KOOK 颜色的最大值的原始值。
    /// </summary>
    public const uint MaxDecimalValue = 0xFFFFFF;

    /// <summary>
    ///     获取默认颜色。
    /// </summary>
    public static readonly Color Default = new(0);

    /// <summary>
    ///     获取青绿色。
    /// </summary>
    /// <remarks>
    ///     青绿色 <c>#1ABC9C</c>（http://www.color-hex.com/color/1abc9c）。
    /// </remarks>
    public static readonly Color Teal = new(0x1ABC9C);

    /// <summary>
    ///     获取深青绿色。
    /// </summary>
    /// <remarks>
    ///     深青绿色 <c>#11806A</c>（http://www.color-hex.com/color/11806a）。
    /// </remarks>
    public static readonly Color DarkTeal = new(0x11806A);

    /// <summary>
    ///     获取绿色。
    /// </summary>
    /// <remarks>
    ///     绿色 <c>#2ECC71</c>（http://www.color-hex.com/color/2ecc71）。
    /// </remarks>
    public static readonly Color Green = new(0x2ECC71);

    /// <summary>
    ///     获取深绿色。
    /// </summary>
    /// <remarks>
    ///     深绿色 <c>#1F8B4C</c>（http://www.color-hex.com/color/1f8b4c）。
    /// </remarks>
    public static readonly Color DarkGreen = new(0x1F8B4C);

    /// <summary>
    ///     获取天蓝色。
    /// </summary>
    /// <remarks>
    ///     天蓝色 <c>#3498DB</c>（http://www.color-hex.com/color/3498db）。
    /// </remarks>
    public static readonly Color Blue = new(0x3498DB);

    /// <summary>
    ///     获取深蓝色。
    /// </summary>
    /// <remarks>
    ///     深蓝色 <c>#206694</c>（http://www.color-hex.com/color/206694）。
    /// </remarks>
    public static readonly Color DarkBlue = new(0x206694);

    /// <summary>
    ///     获取紫色。
    /// </summary>
    /// <remarks>
    ///     紫色 <c>#9B59B6</c>（http://www.color-hex.com/color/9b59b6）。
    /// </remarks>
    public static readonly Color Purple = new(0x9B59B6);

    /// <summary>
    ///     获取深紫色。
    /// </summary>
    /// <remarks>
    ///     深紫色 <c>#71368A</c>（http://www.color-hex.com/color/71368a）。
    /// </remarks>
    public static readonly Color DarkPurple = new(0x71368A);

    /// <summary>
    ///     获取玫瑰红。
    /// </summary>
    /// <remarks>
    ///     玫瑰红 <c>#E91E63</c>（http://www.color-hex.com/color/e91e63）。
    /// </remarks>
    public static readonly Color Magenta = new(0xE91E63);

    /// <summary>
    ///     获取深粉色。
    /// </summary>
    /// <remarks>
    ///     深粉色 <c>#AD1457</c>（http://www.color-hex.com/color/ad1457）。
    /// </remarks>
    public static readonly Color DarkMagenta = new(0xAD1457);

    /// <summary>
    ///     获取金黄色。
    /// </summary>
    /// <remarks>
    ///     金黄色 <c>#F1C40F</c>（http://www.color-hex.com/color/f1c40f）。
    /// </remarks>
    public static readonly Color Gold = new(0xF1C40F);

    /// <summary>
    ///     获取褐橙色。
    /// </summary>
    /// <remarks>
    ///     褐橙色 <c>#C27C0E</c>（http://www.color-hex.com/color/c27c0e）。
    /// </remarks>
    public static readonly Color LightOrange = new(0xC27C0E);

    /// <summary>
    ///     获取橙色。
    /// </summary>
    /// <remarks>
    ///     橙色 <c>#E67E22</c>（http://www.color-hex.com/color/e67e22）。
    /// </remarks>
    public static readonly Color Orange = new(0xE67E22);

    /// <summary>
    ///     获取深橙色。
    /// </summary>
    /// <remarks>
    ///     深橙色 <c>#A84300</c>（http://www.color-hex.com/color/a84300）。
    /// </remarks>
    public static readonly Color DarkOrange = new(0xA84300);

    /// <summary>
    ///     获取猩红色。
    /// </summary>
    /// <remarks>
    ///     猩红色 <c>#E74C3C</c>（http://www.color-hex.com/color/e74c3c）。
    /// </remarks>
    public static readonly Color Red = new(0xE74C3C);

    /// <summary>
    ///     获取深红色。
    /// </summary>
    /// <remarks>
    ///     深红色 <c>#992D22</c>（http://www.color-hex.com/color/992d22）。
    /// </remarks>
    public static readonly Color DarkRed = new(0x992D22);

    /// <summary>
    ///     获取浅灰色。
    /// </summary>
    /// <remarks>
    ///     浅灰色 <c>#95A5A6</c>（http://www.color-hex.com/color/95a5a6）。
    /// </remarks>
    public static readonly Color LightGrey = new(0x95A5A6);

    /// <summary>
    ///     获取暗灰色。
    /// </summary>
    /// <remarks>
    ///     暗灰色 <c>#666D71</c>（http://www.color-hex.com/color/666d71）。
    /// </remarks>
    public static readonly Color Grey = new(0x666D71);

    /// <summary>
    ///     获取钢蓝色。
    /// </summary>
    /// <remarks>
    ///     钢蓝色 <c>#607D8B</c>（http://www.color-hex.com/color/607d8b）。
    /// </remarks>
    public static readonly Color DarkGrey = new(0x607D8B);

    /// <summary>
    ///     获取深青色。
    /// </summary>
    /// <remarks>
    ///     深青色 <c>#3A4B53</c>（http://www.color-hex.com/color/3a4b53）。
    /// </remarks>
    public static readonly Color DarkerGrey = new(0x3A4B53);

    /// <summary>
    ///     获取此颜色的原始值。
    /// </summary>
    /// <remarks>
    ///     颜色以 24 位无符号整型值 RGB 格式进行编码，由高至低的每 8 位分别表示红色、绿色和蓝色通道的强度。
    /// </remarks>
    public uint RawValue { get; }

    /// <summary>
    ///     获取此颜色的红色通道的强度。
    /// </summary>
    public byte R => (byte)(RawValue >> 16);

    /// <summary>
    ///     获取此颜色的绿色通道的强度。
    /// </summary>
    public byte G => (byte)(RawValue >> 8);

    /// <summary>
    ///     获取此颜色的蓝色通道的强度。
    /// </summary>
    public byte B => (byte)RawValue;

    /// <summary>
    ///     使用指定的 24 位无符号整型值初始化一个 <see cref="Color"/> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色：
    ///     <code language="cs">
    ///         Color darkGrey = new Color(0x607D8B);
    ///     </code>
    /// </example>
    /// <param name="rawValue"> 颜色的 24 位无符号整型原始值。 </param>
    /// <exception cref="ArgumentException"> 颜色原始值超过了 <see cref="MaxDecimalValue"/>。 </exception>
    public Color(uint rawValue)
    {
        if (rawValue > MaxDecimalValue)
            throw new ArgumentException($"{nameof(RawValue)} of color cannot be greater than {MaxDecimalValue}!",
                nameof(rawValue));

        RawValue = rawValue;
    }

    /// <summary>
    ///     使用指定的 RGB 通道值初始化一个 <see cref="Color" /> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色：
    ///     <code language="cs">
    ///         Color darkGrey = new Color((byte)0x60, (byte)0x7D, (byte)0x8B);
    ///     </code>
    /// </example>
    /// <param name="r"> 红色通道的强度。 </param>
    /// <param name="g"> 绿色通道的强度。 </param>
    /// <param name="b"> 蓝色通道的强度。 </param>
    /// <exception cref="ArgumentException"> 所提供的三个通道的强度值所组成的颜色的原始值超过了 <see cref="MaxDecimalValue"/>。 </exception>
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
    ///     使用指定的 RGB 通道值初始化一个 <see cref="Color" /> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色：
    ///     <code language="cs">
    ///         Color darkGrey = new Color(96, 125, 139);
    ///     </code>
    /// </example>
    /// <param name="r"> 红色通道的强度。 </param>
    /// <param name="g"> 绿色通道的强度。 </param>
    /// <param name="b"> 蓝色通道的强度。 </param>
    /// <exception cref="ArgumentException"> 所提供的三个通道的强度值所组成的颜色的原始值超过了 <see cref="MaxDecimalValue"/>。 </exception>
    public Color(int r, int g, int b)
    {
        if (r is < 0 or > 255)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,255].");

        if (g is < 0 or > 255)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,255].");

        if (b is < 0 or > 255)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,255].");

        RawValue = ((uint)r << 16)
            | ((uint)g << 8)
            | (uint)b;
    }

    /// <summary>
    ///     使用指定的 RGB 通道值初始化一个 <see cref="Color" /> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色：
    ///     <code language="cs">
    ///         Color darkGrey = new Color(0.38f, 0.49f, 0.55f);
    ///     </code>
    /// </example>
    /// <param name="r"> 红色通道的强度。 </param>
    /// <param name="g"> 绿色通道的强度。 </param>
    /// <param name="b"> 蓝色通道的强度。 </param>
    /// <exception cref="ArgumentException"> 所提供的三个通道的强度值所组成的颜色的原始值超过了 <see cref="MaxDecimalValue"/>。 </exception>
    public Color(float r, float g, float b)
    {
        if (r is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,1].");

        if (g is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,1].");

        if (b is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,1].");

        RawValue = ((uint)(r * 255.0f) << 16)
            | ((uint)(g * 255.0f) << 8)
            | (uint)(b * 255.0f);
    }

    /// <summary>
    ///     判定两个 <see cref="Color"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="Color"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(Color lhs, Color rhs) => lhs.RawValue == rhs.RawValue;

    /// <summary>
    ///     判定两个 <see cref="Color"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="Color"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(Color lhs, Color rhs) => lhs.RawValue != rhs.RawValue;

    /// <summary>
    ///     使用指定的 24 位无符号整型值初始化一个 <see cref="Color"/> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色：
    ///     <code language="cs">
    ///         Color darkGrey = 0x607D8B;
    ///     </code>
    /// </example>
    /// <param name="rawValue"> 颜色的 24 位无符号整型原始值。 </param>
    /// <exception cref="ArgumentException"> 颜色原始值超过了 <see cref="MaxDecimalValue"/>。 </exception>
    public static implicit operator Color(uint rawValue) => new(rawValue);

    /// <inheritdoc cref="P:Kook.Color.RawValue" />
    public static implicit operator uint(Color color) => color.RawValue;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Color c && RawValue == c.RawValue;

    /// <inheritdoc />
    public override int GetHashCode() => RawValue.GetHashCode();

    /// <summary>
    ///     将由 Kook.Net 定义的 <see cref="T:Kook.Color"/> 颜色转换为由 .NET 定义的 <see cref="T:System.Drawing.Color"/> 颜色。
    /// </summary>
    /// <param name="color"> 要进行转换的 <see cref="T:Kook.Color"/> 颜色。 </param>
    /// <returns> 与该 <see cref="T:Kook.Color"/> 颜色具有相同色值的 .NET <see cref="T:System.Drawing.Color"/> 颜色。 </returns>
    public static implicit operator StandardColor(Color color) =>
        StandardColor.FromArgb((int)color.RawValue);

    /// <summary>
    ///     将由 .NET 定义的 <see cref="T:System.Drawing.Color"/> 颜色转换为由 Kook.Net 定义的 <see cref="T:Kook.Color"/> 颜色。
    /// </summary>
    /// <param name="color"> 要进行转换的 .NET <see cref="T:System.Drawing.Color"/> 颜色。 </param>
    /// <returns> 与该 .NET <see cref="T:System.Drawing.Color"/> 颜色具有相同色值的 <see cref="T:Kook.Color"/> 颜色。 </returns>
    public static explicit operator Color(StandardColor color) =>
        new(((uint)color.ToArgb() << 8) >> 8);

    /// <summary>
    ///     获取此颜色带有 <c>#</c> 前缀的 RGB 十六进制字符串表示形式（例如 <c>#000CCC</c>）。
    /// </summary>
    /// <returns> 此颜色带有 <c>#</c> 前缀的 RGB 十六进制字符串表示形式（例如 <c>#000CCC</c>）。 </returns>
    public override string ToString() => $"#{RawValue:X6}";

    private string DebuggerDisplay => $"#{RawValue:X6} ({RawValue})";
}
