using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="ContextModule"/> 模块的构建器。
/// </summary>
public class ContextModuleBuilder : IModuleBuilder, IEquatable<ContextModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <summary>
    ///     元素的最大数量。
    /// </summary>
    public const int MaxElementCount = 10;

    /// <summary>
    ///     初始化一个 <see cref="ContextModuleBuilder"/> 类的新实例。
    /// </summary>
    public ContextModuleBuilder()
    {
        Elements = [];
    }

    /// <summary>
    ///     初始化一个 <see cref="ContextModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="elements"> 备注模块要包含的元素。 </param>
    public ContextModuleBuilder(IList<IElementBuilder> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Context;

    /// <summary>
    ///     获取或设置备注模块的元素。
    /// </summary>
    public IList<IElementBuilder> Elements { get; set; }

    /// <summary>
    ///     添加一个纯文本元素到备注模块。
    /// </summary>
    /// <param name="field"> 要添加的纯文本元素。 </param>
    /// <returns> 当前构建器。 </returns>
    public ContextModuleBuilder AddElement(PlainTextElementBuilder field)
    {
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     添加一个 KMarkdown 元素到备注模块。
    /// </summary>
    /// <param name="field"> 要添加的 KMarkdown 元素。 </param>
    /// <returns> 当前构建器。 </returns>
    public ContextModuleBuilder AddElement(KMarkdownElementBuilder field)
    {
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     添加一个图片元素到备注模块。
    /// </summary>
    /// <param name="field"> 要添加的图片元素。 </param>
    /// <returns> 当前构建器。 </returns>
    public ContextModuleBuilder AddElement(ImageElementBuilder field)
    {
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     添加一个元素到备注模块。
    /// </summary>
    /// <param name="action"> 一个包含对要添加的新创建的元素进行配置的操作的委托。 </param>
    /// <typeparam name="T"> 要添加的元素的类型。 </typeparam>
    /// <returns> 当前构建器。 </returns>
    public ContextModuleBuilder AddElement<T>(Action<T>? action = null)
        where T : IElementBuilder, new()
    {
        T field = new();
        action?.Invoke(field);
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ContextModule"/> 对象。
    /// </summary>
    /// <returns>
    ///     由当前构建器表示的属性构建的 <see cref="ContextModule"/> 对象。
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Elements"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> 的元素数量超过了 <see cref="MaxElementCount"/>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> 包含一个既不是 <see cref="PlainTextElementBuilder"/>，
    ///     也不是 <see cref="KMarkdownElementBuilder"/> 或 <see cref="ImageElementBuilder"/> 的元素。
    /// </exception>
    public ContextModule Build()
    {
        if (Elements is null)
            throw new ArgumentNullException(
                nameof(Elements),
                "Element cannot be null.");

        if (Elements.Count > MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(Elements));

        if (Elements.Any(e => e is not (PlainTextElementBuilder or KMarkdownElementBuilder or ImageElementBuilder)))
            throw new ArgumentException(
                "Elements must be of type PlainText, KMarkdown or Image.",
                nameof(Elements));

        return new ContextModule([..Elements.Select(e => e.Build())]);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="ContextModuleBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ContextModuleBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ContextModuleBuilder? left, ContextModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ContextModuleBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ContextModuleBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ContextModuleBuilder? left, ContextModuleBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ContextModuleBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] ContextModuleBuilder? contextModuleBuilder)
    {
        if (contextModuleBuilder is null)
            return false;

        if (Elements.Count != contextModuleBuilder.Elements.Count)
            return false;

        if (Elements
            .Zip(contextModuleBuilder.Elements, (x, y) => (x, y))
            .Any(pair => pair.x != pair.y))
            return false;

        return Type == contextModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as ContextModuleBuilder);
}
