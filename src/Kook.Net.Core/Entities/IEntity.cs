namespace Kook;

/// <summary>
///     表示一个通用的具有唯一标识符的实体。
/// </summary>
/// <typeparam name="TId"> 唯一标识符的类型。 </typeparam>
public interface IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     获取此实体的唯一标识符。
    /// </summary>
    TId Id { get; }
}
