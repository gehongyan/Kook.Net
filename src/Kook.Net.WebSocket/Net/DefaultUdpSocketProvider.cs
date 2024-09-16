namespace Kook.Net.Udp;

/// <summary>
///     表示一个默认的 <see cref="Kook.Net.Udp.UdpSocketProvider"/>，用于创建
///     <see cref="Kook.Net.Udp.IUdpSocket"/> 的默认实现的实例。
/// </summary>
public static class DefaultUdpSocketProvider
{
    /// <summary>
    ///     获取一个默认的 <see cref="Kook.Net.Udp.UdpSocketProvider"/> 委托，用于创建
    ///     <see cref="Kook.Net.Udp.IUdpSocket"/> 的默认实现的实例。
    /// </summary>
    public static readonly UdpSocketProvider Instance = () =>
    {
        try
        {
            return new DefaultUdpSocket();
        }
        catch (PlatformNotSupportedException ex)
        {
            throw new PlatformNotSupportedException(
                "The default UdpSocketProvider is not supported on this platform.", ex);
        }
    };
}
