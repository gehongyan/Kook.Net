namespace Kook.Net.DependencyInjection.Microsoft;

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
