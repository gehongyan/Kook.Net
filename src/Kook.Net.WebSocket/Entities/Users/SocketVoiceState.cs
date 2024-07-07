using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket user's voice connection status.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct SocketVoiceState : IVoiceState
{
    private readonly ConcurrentDictionary<ulong, SocketVoiceChannel> _voiceChannels;

    /// <summary>
    ///     Initializes a default <see cref="SocketVoiceState"/> with everything set to <c>null</c> or <c>false</c>.
    /// </summary>
    public static SocketVoiceState Default => new();

    /// <summary>
    ///     Initializes a new <see cref="SocketVoiceState"/> with the specified voice channel.
    /// </summary>
    public SocketVoiceState()
    {
        _voiceChannels = [];
    }

    /// <summary>
    ///     Gets the voice channel that the user is currently in; or <c>null</c> if none.
    /// </summary>
    public SocketVoiceChannel? VoiceChannel => _voiceChannels.Values.FirstOrDefault();

    /// <summary>
    ///     Gets a collection of voice channels that the user is connected to.
    /// </summary>
    /// <remarks>
    ///     Currently, KOOK only allows a user to be in one voice channel at a time,
    ///     but allows a Bot user to be in multiple voice channels at a time.
    /// </remarks>
    public IReadOnlyCollection<SocketVoiceChannel> VoiceChannels => [.._voiceChannels.Values];

    /// <inheritdoc />
    public bool? IsMuted { get; private set; }

    /// <inheritdoc />
    public bool? IsDeafened { get; private set; }

    /// <summary>
    ///     Gets the live stream status of the user.
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
    ///     Gets the name of this voice channel.
    /// </summary>
    /// <returns>
    ///     A string that resolves to name of this voice channel; otherwise "Unknown".
    /// </returns>
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
}
