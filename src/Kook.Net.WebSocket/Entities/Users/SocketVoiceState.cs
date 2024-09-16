using System.Collections.Concurrent;
using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的用户的语音连接状态。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct SocketVoiceState : IVoiceState
{
    private readonly ConcurrentDictionary<ulong, SocketVoiceChannel> _voiceChannels;

    /// <inheritdoc cref="Kook.WebSocket.SocketVoiceState()" />
    public static SocketVoiceState Default => new();

    /// <summary>
    ///     初始化一个 <see cref="SocketVoiceState"/> 结构的新实例。
    /// </summary>
    public SocketVoiceState()
    {
        _voiceChannels = [];
    }

    /// <summary>
    ///     获取用户当前所在的语音频道；如果不在任何频道中则为 <c>null</c>。
    /// </summary>
    public SocketVoiceChannel? VoiceChannel => _voiceChannels.Values.FirstOrDefault();

    /// <summary>
    ///     获取用户连接到的所有语音频道。
    /// </summary>
    /// <remarks>
    ///     目前，KOOK 仅允许用户同时连接到一个语音频道，但允许 Bot 用户同时连接到多个语音频道。
    /// </remarks>
    public IReadOnlyCollection<SocketVoiceChannel> VoiceChannels => [.._voiceChannels.Values];

    /// <inheritdoc />
    public bool? IsMuted { get; private set; }

    /// <inheritdoc />
    public bool? IsDeafened { get; private set; }

    /// <summary>
    ///     获取用户的直播状态。
    /// </summary>
    public LiveStreamStatus? LiveStreamStatus { get; private set; }

    internal void Join(SocketVoiceChannel voiceChannel)
    {
        _voiceChannels[voiceChannel.Id] = voiceChannel;
    }

    internal void Leave(ulong id)
    {
        _voiceChannels.TryRemove(id, out _);
    }

    internal void ResetChannels()
    {
        _voiceChannels.Clear();
    }

    internal void Update(bool? isMuted, bool? isDeafened)
    {
        if (isMuted.HasValue)
            IsMuted = isMuted.Value;
        if (isDeafened.HasValue)
            IsDeafened = isDeafened.Value;
    }

    internal void Update(IEnumerable<SocketVoiceChannel> channel)
    {
        ResetChannels();
        foreach (SocketVoiceChannel voiceChannel in channel)
            _voiceChannels[voiceChannel.Id] = voiceChannel;
    }

    internal void Update(SocketVoiceChannel? voiceChannel, API.Gateway.LiveInfo model)
    {
        LiveStreamStatus?.Update(voiceChannel, model);
        LiveStreamStatus ??= Kook.WebSocket.LiveStreamStatus.Create(voiceChannel, model);
    }

    /// <summary>
    ///     获取此语音状态所属语音频道的名称。
    /// </summary>
    public override string ToString() => VoiceChannel?.Name ?? "Unknown";

    private string DebuggerDisplay =>
        $"{VoiceChannel?.Name ?? "Unknown"} ({
            IsMuted switch
            {
                true => "Muted",
                false => "Unmuted",
                _ => "Unknown"
            }} / {
            IsDeafened switch
            {
                true => "Deafened",
                false => "Undeafened",
                _ => "Unknown"
            }})";

    internal SocketVoiceState Clone() => this;

    /// <inheritdoc />
    IVoiceChannel? IVoiceState.VoiceChannel => VoiceChannel;

    /// <inheritdoc />
    IReadOnlyCollection<IVoiceChannel> IVoiceState.VoiceChannels => VoiceChannels;
}
