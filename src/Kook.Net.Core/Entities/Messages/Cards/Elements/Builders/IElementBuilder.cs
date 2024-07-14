namespace Kook;

/// <summary>
///     表示一个通用的元素构建器，用于构建一个 <see cref="IElement"/>。
/// </summary>
public interface IElementBuilder
{
    /// <summary>
    ///     获取此构建器构建的元素的类型。
    /// </summary>
    ElementType Type { get; }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="IElement"/>。
    /// </summary>
    /// <returns>
    ///     由当前构建器表示的属性构建的 <see cref="IElement"/> 对象。
    /// </returns>
    IElement Build();
}
