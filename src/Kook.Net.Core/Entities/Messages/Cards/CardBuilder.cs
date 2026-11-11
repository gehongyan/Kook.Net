using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="Card"/> 卡片的构建器。
/// </summary>
public record CardBuilder : ICardBuilder
{
    /// <summary>
    ///     初始化一个 <see cref="CardBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="theme"> 卡片的主题。 </param>
    /// <param name="color"> 卡片侧边的颜色。 </param>
    /// <param name="size"> 卡片的大小。 </param>
    /// <param name="modules"> 卡片的模块。 </param>
    public CardBuilder(CardTheme theme = CardTheme.Primary, Color? color = null, CardSize size = CardSize.Large,
        IList<IModuleBuilder>? modules = null)
    {
        Theme = theme;
        Color = color;
        Size = size;
        Modules = modules ?? [];
    }

    /// <inheritdoc />
    public CardType Type => CardType.Card;

    /// <summary>
    ///     获取或设置卡片的主题。
    /// </summary>
    /// <remarks>
    ///     <see cref="Color"/> 属性的优先级高于此属性。
    /// </remarks>
    public CardTheme Theme { get; set; }

    /// <summary>
    ///     获取或设置卡片侧边的颜色。
    /// </summary>
    /// <remarks>
    ///     此属性的优先级高于 <see cref="Theme"/> 属性。
    /// </remarks>
    public Color? Color { get; set; }

    /// <summary>
    ///     获取或设置卡片的大小。
    /// </summary>
    public CardSize Size { get; set; }

    /// <summary>
    ///     获取或设置卡片的模块。
    /// </summary>
    public IList<IModuleBuilder> Modules { get; set; }

    /// <summary>
    ///     设置卡片的主题。
    /// </summary>
    /// <param name="theme"> 卡片的主题。 </param>
    /// <returns> 当前构建器。 </returns>
    /// <remarks>
    ///     <see cref="Color"/> 属性的优先级高于 <see cref="Theme"/> 属性。
    /// </remarks>
    public CardBuilder WithTheme(CardTheme theme)
    {
        Theme = theme;
        return this;
    }

    /// <summary>
    ///     设置卡片侧边的颜色。
    /// </summary>
    /// <param name="color"> 卡片侧边的颜色。 </param>
    /// <returns> 当前构建器。 </returns>
    /// <remarks>
    ///     <see cref="Color"/> 属性的优先级高于 <see cref="Theme"/> 属性。
    /// </remarks>
    public CardBuilder WithColor(Color? color)
    {
        Color = color;
        return this;
    }

    /// <summary>
    ///     设置卡片的大小。
    /// </summary>
    /// <param name="size"> 卡片的大小。 </param>
    /// <returns> 当前构建器。 </returns>
    public CardBuilder WithSize(CardSize size)
    {
        Size = size;
        return this;
    }

    /// <summary>
    ///     添加一个模块到卡片。
    /// </summary>
    /// <param name="module"> 要添加的模块。 </param>
    /// <returns> 当前构建器。 </returns>
    public CardBuilder AddModule(IModuleBuilder module)
    {
        Modules.Add(module);
        return this;
    }

    /// <summary>
    ///     添加一个模块到卡片。
    /// </summary>
    /// <param name="action"> 一个包含对要添加的新创建的模块进行配置的操作的委托。 </param>
    /// <returns> 当前构建器。 </returns>
    public CardBuilder AddModule<T>(Action<T>? action = null)
        where T : IModuleBuilder, new()
    {
        T module = new();
        action?.Invoke(module);
        AddModule(module);
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="Card"/>。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="Card"/> 对象。 </returns>
    public Card Build()
    {
        if (Theme is CardTheme.Invisible)
        {
            foreach (IModuleBuilder module in Modules)
            {
                switch (module)
                {
                    case ContextModuleBuilder
                        or ActionGroupModuleBuilder
                        or DividerModuleBuilder
                        or HeaderModuleBuilder
                        or ContainerModuleBuilder
                        or FileModuleBuilder
                        or AudioModuleBuilder
                        or VideoModuleBuilder:
                        break;
                    case SectionModuleBuilder section:
                        if (section.Accessory is not null)
                            throw new InvalidOperationException("SectionModule cannot have an accessory in an card with invisible theme.");
                        break;
                    default:
                        throw new InvalidOperationException($"{module.Type}Module is not supported in an card with invisible theme.");
                }
            }
        }

        return new Card(Theme, Size, Color, [..Modules.Select(m => m.Build())]);
    }

    /// <inheritdoc />
    ICard ICardBuilder.Build() => Build();
}
