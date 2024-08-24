using System.Diagnostics.CodeAnalysis;
using Kook.Utils;

namespace Kook;

/// <summary>
///     用来构建 <see cref="ImageElement"/> 元素的构建器。
/// </summary>
public class ImageElementBuilder : IElementBuilder, IEquatable<ImageElementBuilder>, IEquatable<IElementBuilder>
{
    /// <summary>
    ///     图片替代文本的最大长度。
    /// </summary>
    public const int MaxAlternativeLength = 20;

    /// <summary>
    ///     初始化一个 <see cref="ImageElementBuilder"/> 类的新实例。
    /// </summary>
    public ImageElementBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="ImageElementBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="source"> 图片的源。 </param>
    /// <param name="alternative"> 图片的替代文本。 </param>
    /// <param name="size"> 图片的大小。 </param>
    /// <param name="circle"> 图片是否应渲染为圆形。 </param>
    /// <remarks>
    ///     <paramref name="size"/> 仅在 <see cref="ContextModuleBuilder"/> 中生效，<see cref="ContainerModule"/> 中不生效。
    /// </remarks>
    public ImageElementBuilder(string source, string? alternative = null, ImageSize? size = null, bool circle = false)
    {
        Source = source;
        Alternative = alternative;
        Size = size;
        Circle = circle;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.Image;

    /// <summary>
    ///     获取或设置图片的源。
    /// </summary>
    /// <remarks>
    ///     图片的媒体类型仅支持 <c>image/jpeg</c>、<c>image/gif</c>、<c>image/png</c>。
    /// </remarks>
    public string? Source { get; set; }

    /// <summary>
    ///     获取或设置图片的替代文本。
    /// </summary>
    public string? Alternative { get; set; }

    /// <summary>
    ///     获取或设置 <see cref="ImageElementBuilder"/> 的图片大小。
    /// </summary>
    /// <remarks>
    ///     当前属性仅在 <see cref="ContextModuleBuilder"/> 中生效，<see cref="ContainerModule"/> 中不生效。
    /// </remarks>
    public ImageSize? Size { get; set; }

    /// <summary>
    ///     获取或设置图片是否应渲染为圆形。
    /// </summary>
    public bool? Circle { get; set; }

    /// <summary>
    ///     设置图片的源，值将被设置到 <see cref="Source"/> 属性上。
    /// </summary>
    /// <param name="source"> 图片的源。 </param>
    /// <returns> 当前构建器。 </returns>
    /// <remarks>
    ///     图片的媒体类型仅支持 <c>image/jpeg</c>、<c>image/gif</c>、<c>image/png</c>。
    /// </remarks>
    public ImageElementBuilder WithSource(string? source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     设置图片的替代文本，值将被设置到 <see cref="Alternative"/> 属性上。
    /// </summary>
    /// <param name="alternative"> 图片的替代文本。 </param>
    /// <returns> 当前构建器。 </returns>
    public ImageElementBuilder WithAlternative(string? alternative)
    {
        Alternative = alternative;
        return this;
    }

    /// <summary>
    ///     设置图片的大小，值将被设置到 <see cref="Size"/> 属性上。
    /// </summary>
    /// <param name="size"> 图片的大小。 </param>
    /// <returns> 当前构建器。 </returns>
    public ImageElementBuilder WithSize(ImageSize? size)
    {
        Size = size;
        return this;
    }

    /// <summary>
    ///     设置图片是否应渲染为圆形，值将被设置到 <see cref="Circle"/> 属性上。
    /// </summary>
    /// <param name="circle"> 图片是否应渲染为圆形。 </param>
    /// <returns> 当前构建器。 </returns>
    public ImageElementBuilder WithCircle(bool? circle)
    {
        Circle = circle;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ImageElement"/>。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="ImageElement"/> 对象。 </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Source"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Source"/> 为空字符串。
    /// </exception>
    /// <exception cref="UriFormatException">
    ///     <see cref="Source"/> 不是有效的 URL。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Alternative"/> 的长度超过了 <see cref="MaxAlternativeLength"/>。
    /// </exception>
    [MemberNotNull(nameof(Source))]
    public ImageElement Build()
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source), "The source url cannot be null or empty.");
        if (string.IsNullOrEmpty(Source))
            throw new ArgumentException("The source url cannot be null or empty.", nameof(Source));

        UrlValidation.Validate(Source);

        if (Alternative?.Length > MaxAlternativeLength)
        {
            throw new ArgumentException(
                $"Image alternative length must be less than or equal to {MaxAlternativeLength}.",
                nameof(Alternative));
        }

        return new ImageElement(Source, Alternative, Size, Circle);
    }

    /// <summary>
    ///     使用指定的图片源初始化一个新的 <see cref="ImageElementBuilder"/> 类的实例。
    /// </summary>
    /// <param name="source"> 图片的源。 </param>
    /// <returns> 一个使用指定的图片源初始化的 <see cref="ImageElementBuilder"/> 类的实例。 </returns>
    public static implicit operator ImageElementBuilder(string source) => new(source);

    /// <inheritdoc />
    [MemberNotNull(nameof(Source))]
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="ImageElementBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ImageElementBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ImageElementBuilder? left, ImageElementBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ImageElementBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ImageElementBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ImageElementBuilder? left, ImageElementBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ImageElementBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] ImageElementBuilder? imageElementBuilder)
    {
        if (imageElementBuilder is null) return false;

        return Type == imageElementBuilder.Type
            && Source == imageElementBuilder.Source
            && Alternative == imageElementBuilder.Alternative
            && Size == imageElementBuilder.Size
            && Circle == imageElementBuilder.Circle;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IElementBuilder>.Equals([NotNullWhen(true)] IElementBuilder? elementBuilder) =>
        Equals(elementBuilder as ImageElementBuilder);
}
