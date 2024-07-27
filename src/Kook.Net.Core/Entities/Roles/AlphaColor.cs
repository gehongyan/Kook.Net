using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using StandardColor = System.Drawing.Color;

namespace Kook;

/// <summary>
///     表示 KOOK 中使用的带有不透明度通道的颜色。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct AlphaColor
{
    /// <summary>
    ///     获取一个 KOOK 中带有不透明度通道的颜色的最大值的原始值。
    /// </summary>
    public const uint MaxDecimalValue = 0xFFFFFFFF;

    /// <summary>
    ///     获取默认颜色。
    /// </summary>
    public static readonly AlphaColor Default = new(Color.Default, 0xFF);

    /// <summary>
    ///     获取此颜色的原始值。
    /// </summary>
    /// <remarks>
    ///     颜色以 32 位无符号整型值 RGBA 格式进行编码，由高至低的每 8 位分别表示红色、绿色、蓝色和不透明度通道的强度。
    /// </remarks>
    public uint RawValue { get; }

    /// <summary>
    ///     获取此颜色的红色通道的强度。
    /// </summary>
    public byte R => (byte)(RawValue >> 24);

    /// <summary>
    ///     获取此颜色的绿色通道的强度。
    /// </summary>
    public byte G => (byte)(RawValue >> 16);

    /// <summary>
    ///     获取此颜色的蓝色通道的强度。
    /// </summary>
    public byte B => (byte)(RawValue >> 8);

    /// <summary>
    ///     获取此颜色的不透明度通道的强度。
    /// </summary>
    public byte A => (byte)RawValue;

    /// <summary>
    ///     获取此颜色不带有不透明度通道的基础颜色。
    /// </summary>
    public Color BaseColor => new(RawValue >> 8);

    /// <summary>
    ///     使用指定的 32 位无符号整型值初始化一个 <see cref="AlphaColor"/> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8BFF（http://www.color-hex.com/color/607d8b）所表示的颜色，且其完全不透明：
    ///     <code language="cs">
    ///         AlphaColor darkGrey = new AlphaColor(0x607D8BFF);
    ///     </code>
    /// </example>
    /// <param name="rawValue"> 颜色的 32 位无符号整型原始值。 </param>
    public AlphaColor(uint rawValue)
    {
        RawValue = rawValue;
    }

    /// <summary>
    ///     使用指定的 <see cref="T:Kook.Color"/> 及不透明度初始化一个 <see cref="AlphaColor"/> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8BFF（http://www.color-hex.com/color/607d8b）所表示的颜色，且其完全不透明：
    ///     <code language="cs">
    ///         AlphaColor darkGrey = new AlphaColor(new Color(0x607D8B), (byte)0xFF);
    ///     </code>
    /// </example>
    /// <param name="baseColor"> 基础颜色。 </param>
    /// <param name="alpha"> 不透明度。 </param>
    public AlphaColor(Color baseColor, byte alpha)
    {
        uint value = ((uint)baseColor.R << 24)
            | ((uint)baseColor.G << 16)
            | ((uint)baseColor.B << 8)
            | (uint)alpha;

        RawValue = value;
    }

    /// <summary>
    ///     使用指定的 RGBA 通道值初始化一个 <see cref="Color" /> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色，且其完全不透明：
    ///     <code language="cs">
    ///         AlphaColor darkGrey = new AlphaColor((byte)0x60, (byte)0x7D, (byte)0x8B, (byte)0xFF);
    ///     </code>
    /// </example>
    /// <param name="r"> 红色通道的强度。 </param>
    /// <param name="g"> 绿色通道的强度。 </param>
    /// <param name="b"> 蓝色通道的强度。 </param>
    /// <param name="a"> 不透明度通道的强度。 </param>
    public AlphaColor(byte r, byte g, byte b, byte a)
    {
        uint value = ((uint)r << 24)
            | ((uint)g << 16)
            | ((uint)b << 8)
            | (uint)a;

        RawValue = value;
    }

    /// <summary>
    ///     使用指定的 RGBA 通道值初始化一个 <see cref="Color" /> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色，且其完全不透明：
    ///     <code language="cs">
    ///         AlphaColor darkGrey = new AlphaColor(96, 125, 139, 255);
    ///     </code>
    /// </example>
    /// <param name="r"> 红色通道的强度。 </param>
    /// <param name="g"> 绿色通道的强度。 </param>
    /// <param name="b"> 蓝色通道的强度。 </param>
    /// <param name="a"> 不透明度通道的强度。 </param>
    public AlphaColor(int r, int g, int b, int a)
    {
        if (r is < 0 or > 255)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,255].");

        if (g is < 0 or > 255)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,255].");

        if (b is < 0 or > 255)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,255].");

        if (a is < 0 or > 255)
            throw new ArgumentOutOfRangeException(nameof(a), "Value must be within [0,255].");

        RawValue = ((uint)r << 24)
            | ((uint)g << 16)
            | ((uint)b << 8)
            | (uint)a;
    }

    /// <summary>
    ///     使用指定的 RGBA 通道值初始化一个 <see cref="Color" /> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色，且其完全不透明：
    ///     <code language="cs">
    ///         AlphaColor darkGrey = new AlphaColor(0.38f, 0.49f, 0.55f, 1.00f);
    ///     </code>
    /// </example>
    /// <param name="r"> 红色通道的强度。 </param>
    /// <param name="g"> 绿色通道的强度。 </param>
    /// <param name="b"> 蓝色通道的强度。 </param>
    /// <param name="a"> 不透明度通道的强度。 </param>
    public AlphaColor(float r, float g, float b, float a)
    {
        if (r is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(r), "Value must be within [0,1].");

        if (g is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(g), "Value must be within [0,1].");

        if (b is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(b), "Value must be within [0,1].");

        if (a is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(a), "Value must be within [0,1].");

        RawValue = ((uint)(r * 255.0f) << 24)
            | ((uint)(g * 255.0f) << 16)
            | ((uint)(b * 255.0f) << 8)
            | (uint)(a * 255.0f);
    }

    /// <summary>
    ///     判定两个 <see cref="AlphaColor"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="AlphaColor"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(AlphaColor lhs, AlphaColor rhs) => lhs.RawValue == rhs.RawValue;

    /// <summary>
    ///     判定两个 <see cref="AlphaColor"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="AlphaColor"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(AlphaColor lhs, AlphaColor rhs) => lhs.RawValue != rhs.RawValue;

    /// <summary>
    ///     使用指定的 32 位无符号整型值初始化一个 <see cref="AlphaColor"/> 结构的新实例。
    /// </summary>
    /// <example>
    ///     创建 #607D8B（http://www.color-hex.com/color/607d8b）所表示的颜色，且其完全不透明：
    ///     <code language="cs">
    ///         AlphaColor darkGrey = 0x607D8BFF;
    ///     </code>
    /// </example>
    /// <param name="rawValue"> 颜色的 32 位无符号整型原始值。 </param>
    public static implicit operator AlphaColor(uint rawValue) => new(rawValue);

    /// <inheritdoc cref="P:Kook.AlphaColor.RawValue" />
    public static implicit operator uint(AlphaColor color) => color.RawValue;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is AlphaColor c && RawValue == c.RawValue;

    /// <inheritdoc />
    public override int GetHashCode() => RawValue.GetHashCode();

    /// <summary>
    ///     Converts the given Kook.Net-defined <see cref="Color"/> to a Kook.Net-defined <see cref="AlphaColor"/>.
    /// </summary>
    /// <param name="color"> The Kook.Net-defined <see cref="Color"/> to convert. </param>
    /// <returns> The Kook.Net-defined <see cref="AlphaColor"/> that represents the given Kook.Net-defined <see cref="Color"/>. </returns>
    public static implicit operator AlphaColor(Color color) =>
        new((color.RawValue << 8) | 0xFF);

    /// <inheritdoc cref="P:Kook.AlphaColor.BaseColor" />
    /// <remarks>
    ///     <note type="warning">
    ///         此转换会丢失不透明度通道的信息。
    ///     </note>
    /// </remarks>
    public static explicit operator Color(AlphaColor color) => color.BaseColor;

    /// <summary>
    ///     将由 Kook.Net 定义的 <see cref="T:Kook.AlphaColor"/> 颜色转换为由 .NET 定义的 <see cref="T:System.Drawing.Color"/> 颜色。
    /// </summary>
    /// <param name="color"> 要进行转换的 <see cref="T:Kook.AlphaColor"/> 颜色。 </param>
    /// <returns> 与该 <see cref="T:Kook.AlphaColor"/> 颜色具有相同色值的 .NET <see cref="T:System.Drawing.Color"/> 颜色。 </returns>
    public static implicit operator StandardColor(AlphaColor color) =>
        StandardColor.FromArgb(color.A, color.R, color.G, color.B);

    /// <summary>
    ///     将由 .NET 定义的 <see cref="T:System.Drawing.Color"/> 颜色转换为由 Kook.Net 定义的 <see cref="T:Kook.AlphaColor"/> 颜色。
    /// </summary>
    /// <param name="color"> 要进行转换的 .NET <see cref="T:System.Drawing.Color"/> 颜色。 </param>
    /// <returns> 与该 .NET <see cref="T:System.Drawing.Color"/> 颜色具有相同色值的 <see cref="T:Kook.AlphaColor"/> 颜色。 </returns>
    public static explicit operator AlphaColor(StandardColor color) =>
        new(color.R, color.G, color.B, color.A);

    /// <summary>
    ///     获取此颜色带有 <c>#</c> 前缀的 RGBA 十六进制字符串表示形式（例如 <c>#000CCCFF</c>）。
    /// </summary>
    /// <returns> 此颜色带有 <c>#</c> 前缀的 RGBA 十六进制字符串表示形式（例如 <c>#000CCCFF</c>）。 </returns>
    public override string ToString() => $"#{RawValue:X8}";

    private string DebuggerDisplay => $"#{RawValue:X8} ({RawValue})";
}
