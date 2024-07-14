namespace Kook;

/// <summary>
///     表示一个通用的用于模块内的元素。
/// </summary>
public interface IElement
{
    /// <summary>
    ///     获取元素的类型。
    /// </summary>
    ElementType Type { get; }
}
