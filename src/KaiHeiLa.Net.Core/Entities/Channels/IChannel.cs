namespace KaiHeiLa;

/// <summary>
///     频道
/// </summary>
public interface IChannel : IULongEntity
{
    /// <summary>
    ///     频道名称
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     创建者 ID
    /// </summary>
    uint CreateUserId { get; }
}