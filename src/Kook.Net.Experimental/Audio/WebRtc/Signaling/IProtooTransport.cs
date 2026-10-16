namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     protoo 的文本帧传输。事件只允许单个订阅者（<see cref="ProtooPeer"/>）。
/// </summary>
internal interface IProtooTransport : IDisposable
{
    event Func<string, Task>? TextReceived;

    event Func<ProtooCloseInfo, Task>? Closed;

    Task ConnectAsync(Uri uri, string subProtocol, CancellationToken cancellationToken);

    Task SendTextAsync(string text, CancellationToken cancellationToken);

    Task CloseAsync(CancellationToken cancellationToken);
}
