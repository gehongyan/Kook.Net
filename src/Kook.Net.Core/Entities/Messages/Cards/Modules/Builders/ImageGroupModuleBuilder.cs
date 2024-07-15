using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="ImageGroupModule"/> 模块的构建器。
/// </summary>
public class ImageGroupModuleBuilder : IModuleBuilder, IEquatable<ImageGroupModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <summary>
    ///     元素的最大数量。
    /// </summary>
    public const int MaxElementCount = 9;

    /// <summary>
    ///     初始化一个 <see cref="ImageGroupModuleBuilder"/> 类的新实例。
    /// </summary>
    public ImageGroupModuleBuilder()
    {
        Elements = [];
    }

    /// <summary>
    ///     初始化一个 <see cref="ImageGroupModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="elements"> 图片组模块要包含的图片元素。 </param>
    public ImageGroupModuleBuilder(IList<ImageElementBuilder> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.ImageGroup;

    /// <summary>
    ///     获取或设置图片组模块的图片元素。
    /// </summary>
    public IList<ImageElementBuilder> Elements { get; set; }

    /// <summary>
    ///     添加一个图片元素到图片组模块。
    /// </summary>
    /// <param name="field"> 要添加的图片元素。 </param>
    /// <returns> 当前构建器。 </returns>
    public ImageGroupModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(field));

        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     添加一个图片元素到图片组模块。
    /// </summary>
    /// <param name="action"> 一个包含对要添加的新创建的图片元素进行配置的操作的委托。 </param>
    /// <returns> 当前构建器。 </returns>
    public ImageGroupModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ImageGroupModule"/> 对象。
    /// </summary>
    /// <returns>
    ///     由当前构建器表示的属性构建的 <see cref="ImageGroupModule"/> 对象。
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Elements"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> 为空列表。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> 的元素数量超过了 <see cref="MaxElementCount"/>。
    /// </exception>
    public ImageGroupModule Build()
    {
        if (Elements is null)
            throw new ArgumentNullException(nameof(Elements), "Element cannot be null.");

        if (Elements.Count == 0)
            throw new ArgumentException("Element cannot be an empty list.", nameof(Elements));

        if (Elements.Count > MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(Elements));

        return new ImageGroupModule([..Elements.Select(e => e.Build())]);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="ImageGroupModuleBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ImageGroupModuleBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ImageGroupModuleBuilder? left, ImageGroupModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ImageGroupModuleBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ImageGroupModuleBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ImageGroupModuleBuilder? left, ImageGroupModuleBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ImageGroupModuleBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] ImageGroupModuleBuilder? imageGroupModuleBuilder)
    {
        if (imageGroupModuleBuilder is null)
            return false;

        if (Elements.Count != imageGroupModuleBuilder.Elements.Count)
            return false;

        if (Elements
            .Zip(imageGroupModuleBuilder.Elements, (x, y) => (x, y))
            .Any(pair => pair.x != pair.y))
            return false;

        return Type == imageGroupModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as ImageGroupModuleBuilder);
}
