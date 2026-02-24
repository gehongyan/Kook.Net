namespace Kook;

/// <summary>
///     用来构建 <see cref="ActionGroupModule"/> 模块的构建器。
/// </summary>
public record ActionGroupModuleBuilder : IModuleBuilder
{
    /// <summary>
    ///     元素的最大数量。
    /// </summary>
    public const int MaxElementCount = 4;

    /// <summary>
    ///     初始化一个 <see cref="ActionGroupModuleBuilder"/> 类的新实例。
    /// </summary>
    public ActionGroupModuleBuilder()
    {
        Elements = [];
    }

    /// <summary>
    ///     初始化一个 <see cref="ActionGroupModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="elements"> 按钮组模块要包含的按钮元素。 </param>
    public ActionGroupModuleBuilder(IList<ButtonElementBuilder> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.ActionGroup;

    /// <summary>
    ///     获取或设置按钮组模块的按钮元素。
    /// </summary>
    public IList<ButtonElementBuilder> Elements { get; set; }

    /// <summary>
    ///     添加一个按钮元素到按钮组模块。
    /// </summary>
    /// <param name="field"> 要添加的按钮元素。 </param>
    /// <returns> 当前构建器。 </returns>
    public ActionGroupModuleBuilder AddElement(ButtonElementBuilder field)
    {
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     添加一个按钮元素到按钮组模块。
    /// </summary>
    /// <param name="action"> 一个包含对要添加的新创建的按钮元素进行配置的操作的委托。 </param>
    /// <returns> 当前构建器。 </returns>
    public ActionGroupModuleBuilder AddElement(Action<ButtonElementBuilder>? action = null)
    {
        ButtonElementBuilder field = new();
        action?.Invoke(field);
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ActionGroupModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="ActionGroupModule"/> 对象。 </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Elements"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> 是一个空列表。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> 的元素数量超过了 <see cref="MaxElementCount"/>。
    /// </exception>
    public ActionGroupModule Build()
    {
        if (Elements == null)
            throw new ArgumentNullException(
                nameof(Elements), "Element cannot be null or empty list.");
        if (Elements.Count == 0)
            throw new ArgumentException(
                "Element cannot be null or empty list.", nameof(Elements));
        if (Elements.Count > MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.", nameof(Elements));
        return new ActionGroupModule([..Elements.Select(e => e.Build())]);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}
