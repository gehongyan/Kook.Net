namespace KaiHeiLa;

/// <summary>
///     Represents a generic voice channel in a guild.
/// </summary>
public interface IVoiceChannel : INestedChannel, IAudioChannel, IMentionable
{
    /// <summary>
    ///     Gets the voice quality that the clients in this voice channel are requested to use.
    /// </summary>
    /// <returns>
    ///     A <see cref="VoiceQuality"/> representing the voice quality that this voice channel defines and requests the
    ///     client(s) to use.
    /// </returns>
    VoiceQuality? VoiceQuality { get; }
    
    /// <summary>
    ///     Gets the max number of users allowed to be connected to this channel at once.
    /// </summary>
    /// <returns>
    ///     An int representing the maximum number of users that are allowed to be connected to this
    ///     channel at once; <c>null</c> if a limit is not set.
    /// </returns>
    int? UserLimit { get; }

    /// <summary>
    ///     Gets the server url that clients should connect to to join this voice channel.
    /// </summary>
    /// <returns>
    ///     A string representing the url that clients should connect to to join this voice channel.
    /// </returns>
    string ServerUrl { get; }
    
    /// <summary>
    ///     Gets whether this voice channel is locked by a password.
    /// </summary>
    /// <returns>
    ///     A bool representing whether this voice channel is locked by a password.
    /// </returns>
    bool HasPassword { get; }
    
    /// <summary>
    ///     Modifies this voice channel.
    /// </summary>
    /// <param name="func">The properties to modify the channel with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    /// <seealso cref="ModifyVoiceChannelProperties"/>
    Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions options = null);
}