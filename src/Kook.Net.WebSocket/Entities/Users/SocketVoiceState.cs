using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket user's voice connection status.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct SocketVoiceState : IVoiceState
{
    /// <summary>
    ///     Initializes a default <see cref="SocketVoiceState"/> with everything set to <c>null</c> or <c>false</c>.
    /// </summary>
    public static readonly SocketVoiceState Default = new(null, null, null);

    /// <summary>
    ///     Initializes a new <see cref="SocketVoiceState"/> with the specified voice channel.
    /// </summary>
    /// <param name="voiceChannel"> The voice channel that the user is currently in. </param>
    /// <param name="isMuted"> Whether the user is muted. </param>
    /// <param name="isDeafened"> Whether the user is deafened. </param>
    public SocketVoiceState(SocketVoiceChannel voiceChannel, bool? isMuted, bool? isDeafened)
    {
        VoiceChannel = voiceChannel;
        IsMuted = isMuted;
        IsDeafened = isDeafened;
    }

    /// <summary>
    ///     Gets the voice channel that the user is currently in; or <c>null</c> if none.
    /// </summary>
    public SocketVoiceChannel VoiceChannel { get; private set; }

    /// <inheritdoc />
    public bool? IsMuted { get; private set; }

    /// <inheritdoc />
    public bool? IsDeafened { get; private set; }

    internal void Update(SocketVoiceChannel voiceChannel) => VoiceChannel = voiceChannel;

    internal void Update(bool? isMuted, bool? isDeafened)
    {
        if (isMuted.HasValue) IsMuted = isMuted.Value;

        if (isDeafened.HasValue) IsDeafened = isDeafened.Value;
    }

    /// <summary>
    ///     Gets the name of this voice channel.
    /// </summary>
    /// <returns>
    ///     A string that resolves to name of this voice channel; otherwise "Unknown".
    /// </returns>
    public override string ToString() => VoiceChannel?.Name ?? "Unknown";

    private string DebuggerDisplay =>
        $"{VoiceChannel?.Name ?? "Unknown"} ({(IsMuted.HasValue ? IsMuted.Value ? "Muted" : "Unmuted" : "Unknown")} / {(IsDeafened.HasValue ? IsDeafened.Value ? "Deafened" : "Undeafened" : "Unknown")})";

    internal SocketVoiceState Clone() => this;

    /// <inheritdoc />
    IVoiceChannel? IVoiceState.VoiceChannel => VoiceChannel;
}
