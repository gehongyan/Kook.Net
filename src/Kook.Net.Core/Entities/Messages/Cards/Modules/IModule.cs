namespace Kook;

/// <summary>
///     Represents a generic module that can be used in an <see cref="ICard"/>.
/// </summary>
public interface IModule
{
    /// <summary>
    ///     Gets the type of the module.
    /// </summary>
    /// <returns>
    ///     A <see cref="ModuleType"/> value that represents the type of the module.
    /// </returns>
    ModuleType Type { get; }
}
