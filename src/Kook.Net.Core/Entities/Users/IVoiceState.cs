namespace Kook;

/// <summary>
///     Represents a user's voice connection status.
/// </summary>
public interface IVoiceState
{
    /// <summary>
    ///     Gets a value that indicates whether this user is deafened by the guild.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the user is deafened (i.e. not permitted to listen to or speak to others) by the guild;
    ///     otherwise <c>false</c>; <c>null</c> if unknown.
    /// </returns>
    bool? IsDeafened { get; }

    /// <summary>
    ///     Gets a value that indicates whether this user is muted (i.e. not permitted to speak via voice) by the
    ///     guild.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if this user is muted by the guild; otherwise <c>false</c>; <c>null</c> if unknown.
    /// </returns>
    bool? IsMuted { get; }

    /// <summary>
    ///     Gets the voice channel this user is currently in.
    /// </summary>
    /// <returns>
    ///     A generic voice channel object representing the voice channel that the user is currently in; <c>null</c>
    ///     if none.
    /// </returns>
    IVoiceChannel VoiceChannel { get; }
}
