namespace Kook;

/// <summary>
///     表示一个通用的模块，可用于 <see cref="ICard"/> 中。
/// </summary>
public interface IModule
{
    /// <summary>
    ///     获取模块的类型。
    /// </summary>
    ModuleType Type { get; }
}
