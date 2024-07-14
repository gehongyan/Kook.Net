namespace Kook;

/// <summary>
///     A generic module builder for creating an <see cref="IModule"/>.
/// </summary>
public interface IModuleBuilder
{
    /// <summary>
    ///     Specifies the module type of the <see cref="IModule"/> this builder creates.
    /// </summary>
    /// <returns>
    ///     A <see cref="ModuleType"/> representing the module type of the <see cref="IModule"/> this builder creates.
    /// </returns>
    ModuleType Type { get; }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="IModule"/>。
    /// </summary>
    /// <returns>
    ///     An <see cref="IModule"/> representing the built module object.
    /// </returns>
    IModule Build();
}
