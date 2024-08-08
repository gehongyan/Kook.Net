namespace Kook;

/// <summary>
///     表示一个消息中通用的标签。
/// </summary>
public interface ITag
{
    /// <summary>
    ///     获取消息中标签的位置。
    /// </summary>
    int Index { get; }

    /// <summary>
    ///     获取标签的长度。
    /// </summary>
    int Length { get; }

    /// <summary>
    ///     获取标签的类型。
    /// </summary>
    TagType Type { get; }

    /// <summary>
    ///     获取标签的键。
    /// </summary>
    object Key { get; }

    /// <summary>
    ///     获取标签的值。
    /// </summary>
    /// <remarks>
    ///     当 <see cref="Type"/> 为 <see cref="TagType.HereMention"/>，此属性应返回表示提及在线成员的实体，但这样的实体不存在。
    ///     为了便利，此属性返回与 <see cref="P:Kook.IGuild.EveryoneRole"/> 相同的实体，但并不表示此标签提及的是所有人，而仍表示提及的是在线成员。
    /// </remarks>
    object? Value { get; }
}
