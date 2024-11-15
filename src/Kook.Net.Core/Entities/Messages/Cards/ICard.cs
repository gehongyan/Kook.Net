namespace Kook;

/// <summary>
///     表示一个通用的卡片。
/// </summary>
public interface ICard
{
    /// <summary>
    ///     获取卡片的类型。
    /// </summary>
    CardType Type { get; }

    /// <summary>
    ///     获取卡片中的模块。
    /// </summary>
    IReadOnlyCollection<IModule> Modules { get; }

    /// <summary>
    ///     获取卡片中模块的数量。
    /// </summary>
    int ModuleCount { get; }
}
