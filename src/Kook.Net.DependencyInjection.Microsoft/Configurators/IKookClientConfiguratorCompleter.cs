namespace Kook.Net.DependencyInjection.Microsoft;

/// <summary>
///     表示一个通用的 KOOK 客户端配置器的完成器。
/// </summary>
public interface IKookClientConfiguratorCompleter
{
    /// <summary>
    ///     完成配置。
    /// </summary>
    void Complete();
}
