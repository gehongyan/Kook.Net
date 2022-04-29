namespace KaiHeiLa;

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
    ///     Builds the <see cref="IModuleBuilder"/> into an <see cref="IModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="IModule"/> representing the built module object.
    /// </returns>
    IModule Build();
}