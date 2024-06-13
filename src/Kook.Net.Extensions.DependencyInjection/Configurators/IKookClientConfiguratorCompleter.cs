namespace Kook.Net.Extensions.DependencyInjection;

/// <summary>
///     Represents a generic completer for a Kook client configurator.
/// </summary>
public interface IKookClientConfiguratorCompleter
{
    /// <summary>
    ///     Completes the configuration.
    /// </summary>
    void Complete();
}
