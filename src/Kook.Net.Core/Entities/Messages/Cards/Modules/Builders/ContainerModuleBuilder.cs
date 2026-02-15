namespace Kook;

/// <summary>
///     用来构建 <see cref="ContainerModule"/> 模块的构建器。
/// </summary>
public record ContainerModuleBuilder : IModuleBuilder
{
    /// <summary>
    ///     元素的最大数量。
    /// </summary>
    public const int MaxElementCount = 9;

    /// <summary>
    ///     初始化一个 <see cref="ContainerModuleBuilder"/> 类的新实例。
    /// </summary>
    public ContainerModuleBuilder()
    {
        Elements = [];
    }

    /// <summary>
    ///     初始化一个 <see cref="ContainerModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="elements"> 图片组模块要包含的图片元素。 </param>
    public ContainerModuleBuilder(IList<ImageElementBuilder> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Container;

    /// <summary>
    ///     获取或设置图片组模块的图片元素。
    /// </summary>
    public IList<ImageElementBuilder> Elements { get; set; }

    /// <summary>
    ///     添加一个图片元素到图片组模块。
    /// </summary>
    /// <param name="field"> 要添加的图片元素。 </param>
    /// <returns> 当前构建器。 </returns>
    public ContainerModuleBuilder AddElement(ImageElementBuilder field)
    {
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     添加一个图片元素到图片组模块。
    /// </summary>
    /// <param name="action"> 一个包含对要添加的新创建的图片元素进行配置的操作的委托。 </param>
    /// <returns> 当前构建器。 </returns>
    public ContainerModuleBuilder AddElement(Action<ImageElementBuilder>? action = null)
    {
        ImageElementBuilder field = new();
        action?.Invoke(field);
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ContainerModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="ContainerModule"/> 对象。 </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Elements"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> 为空列表。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> 的元素数量超过了 <see cref="MaxElementCount"/>。
    /// </exception>
    public ContainerModule Build()
    {
        if (Elements is null)
            throw new ArgumentNullException(nameof(Elements), "Element cannot be null.");

        if (Elements.Count == 0)
            throw new ArgumentException("Element cannot be an empty list.", nameof(Elements));

        if (Elements.Count > MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(Elements));

        return new ContainerModule([..Elements.Select(e => e.Build())]);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}
