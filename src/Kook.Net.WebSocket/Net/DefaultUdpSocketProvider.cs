namespace Kook.Net.Udp;

/// <summary>
///     Represents a delegate that provides a <see cref="IUdpSocket"/> instance.
/// </summary>
public static class DefaultUdpSocketProvider
{
    /// <summary>
    ///     A delegate that creates a default <see cref="UdpSocketProvider"/> instance.
    /// </summary>
    public static readonly UdpSocketProvider Instance = () =>
    {
        try
        {
            return new DefaultUdpSocket();
        }
        catch (PlatformNotSupportedException ex)
        {
            throw new PlatformNotSupportedException("The default UdpSocketProvider is not supported on this platform.", ex);
        }
    };
}
