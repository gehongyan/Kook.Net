namespace Kook;

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