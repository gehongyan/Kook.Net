namespace Kook.Audio;

/// <summary>
///     Represents the information of a peer's permission in a voice channel.
/// </summary>
public readonly struct PeerPermissionInfo
{
    /// <summary>
    ///     Gets whether the peer is muted by the guild.
    /// </summary>
    public bool MutedByGuild { get; init; }

    /// <summary>
    ///     Gets whether the peer is deafened by the guild.
    /// </summary>
    public bool DeafenedByGuild { get; init; }

    /// <summary>
    ///     Gets whether the peer can use voice activity, i.e. the peer can speak without pressing a key.
    /// </summary>
    public bool CanUseVoiceActivity { get; init; }

    /// <summary>
    ///     Gets whether the peer can connect to the voice channel.
    /// </summary>
    public bool CanConnect { get; init; }

    /// <summary>
    ///     Gets whether the peer can speak in the voice channel.
    /// </summary>
    public bool CanSpeak { get; init; }

    /// <summary>
    ///     Gets whether the peer can manage the voice channel.
    /// </summary>
    public bool CanManageVoice { get; init; }
}

