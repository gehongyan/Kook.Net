namespace Kook;

/// <summary>
///     表示一个通用的模块构建器，用于构建一个 <see cref="IModule"/>。
/// </summary>
public interface IModuleBuilder
{
    /// <summary>
    ///     获取此构建器构建的模块的类型。
    /// </summary>
    ModuleType Type { get; }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="IModule"/>。
    /// </summary>
    /// <returns>
    ///     由当前构建器表示的属性构建的 <see cref="IModule"/> 对象。
    /// </returns>
    IModule Build();
}
